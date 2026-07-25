using Microsoft.AspNetCore.Mvc;

namespace LundBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class TrafficController : BaseController
    {
        [HttpPost("visit")]
        public IActionResult VisitedWebsite()
        {
            // TODO: Hash ip, store in database, ensure bot updates message in discord channel
            string ipAddress = getRequestorIpAddress(Request);

            return Ok();
        }

        [HttpPost("invite-click")]
        public IActionResult ClickedInviteLink()
        {
            // TODO: Hash ip, store in database, ensure bot updates message in discord channel to show the click
            string ipAddress = getRequestorIpAddress(Request);

            return Ok();
        }
    }
}
