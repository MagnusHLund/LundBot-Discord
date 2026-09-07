using LundBot.Application.Features.WebsiteTraffic;
using LundBot.Presentation.Api.Common;
using LundBot.Presentation.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LundBot.Presentation.Api.Traffic
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class TrafficController : AbstractBaseController
    {
        private readonly IWebsiteTrafficService _websiteTrafficService;

        public TrafficController(
            IOptions<DeveloperEnvironmentConfig> devConfig,
            IWebsiteTrafficService websiteTrafficService
        )
            : base(devConfig)
        {
            _websiteTrafficService = websiteTrafficService;
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
    }
}
