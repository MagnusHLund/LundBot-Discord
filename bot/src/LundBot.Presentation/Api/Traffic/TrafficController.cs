using LundBot.Application.Features.WebsiteTraffic;
using LundBot.Presentation.Api.Common;
using LundBot.Presentation.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace LundBot.Presentation.Api.Traffic
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class TrafficController : AbstractBaseController
    {
        private readonly IWebsiteTrafficService _websiteTrafficService;
        private readonly DeveloperEnvironmentConfig _devConfig;
        private readonly IHostEnvironment _hostEnvironment;

        public TrafficController(
            IOptions<DeveloperEnvironmentConfig> devConfig,
            IWebsiteTrafficService websiteTrafficService,
            IHostEnvironment hostEnvironment
        )
        {
            _devConfig = devConfig.Value;
            _websiteTrafficService = websiteTrafficService;
            _hostEnvironment = hostEnvironment;
        }

        [HttpPost("visit")]
        public async Task<IActionResult> VisitedWebsite()
        {
            string ipAddress = GetRequestorIpAddress(Request);
            bool success = await _websiteTrafficService.RegisterWebsiteVisitAsync(ipAddress);

            if (success)
            {
                return Ok(new { message = "Website visit registered successfully." });
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPost("invite-click")]
        public async Task<IActionResult> ClickedInviteLink()
        {
            string ipAddress = GetRequestorIpAddress(Request);
            bool success = await _websiteTrafficService.RegisterInviteLinkClickAsync(ipAddress);

            if (success)
            {
                return Ok(new { message = "Invite link click registered successfully." });
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        private string GetRequestorIpAddress(HttpRequest request)
        {
            if (_hostEnvironment.IsDevelopment() && _devConfig.GenerateIpAddresses)
            {
                Random random = new Random();
                return $"{random.Next(1, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(1, 255)}";
            }

            if (
                request.Headers.TryGetValue("X-Forwarded-For", out StringValues forwarded)
                && !StringValues.IsNullOrEmpty(forwarded)
            )
            {
                string? first = forwarded.ToString().Split(',').Select(s => s.Trim()).FirstOrDefault();
                if (!string.IsNullOrEmpty(first))
                {
                    return first;
                }
            }

            var remoteIp = request.HttpContext.Connection.RemoteIpAddress;
            if (remoteIp != null)
            {
                return remoteIp.ToString();
            }

            return "0.0.0.0";
        }
    }
}
