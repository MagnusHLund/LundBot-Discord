using LundBot.Config;
using LundBot.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LundBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class TrafficController : BaseController
    {
        private readonly IWebsiteTrafficService _websiteTrafficService;

        public TrafficController(
            IWebsiteTrafficService websiteTrafficService,
            IOptions<DeveloperEnvironmentConfig> devConfig
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
                return Ok();
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
                return Ok();
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
