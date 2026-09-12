using LundBot.Application.Features.WebsiteTraffic;
using LundBot.Presentation.Api.Common;
using LundBot.Presentation.Api.Traffic.Dtos;
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
        public async Task<IActionResult> VisitedWebsite([FromBody] TrafficRequestDto requestDto)
        {
            string ipAddress = GetRequestorIpAddress(Request);
            bool success = await _websiteTrafficService.RegisterWebsiteVisitAsync(ipAddress, requestDto.GuildId);

            if (!success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return NoContent();
        }

        [HttpPost("invite-click")]
        public async Task<IActionResult> ClickedInviteLink([FromBody] TrafficRequestDto requestDto)
        {
            string ipAddress = GetRequestorIpAddress(Request);
            bool success = await _websiteTrafficService.RegisterInviteLinkClickAsync(ipAddress, requestDto.GuildId);

            if (!success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return NoContent();
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
