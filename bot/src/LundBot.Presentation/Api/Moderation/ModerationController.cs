using LundBot.Presentation.Api.Common;
using LundBot.Presentation.Api.Moderation.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LundBot.Presentation.Api.Moderation
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModerationController : AbstractBaseController
    {
        [Authorize]
        [HttpPost("kick/roles/assign")]
        public IActionResult AssignRoleToAutomaticallyKick([FromBody] RoleToKickRequestDto request)
        {
            bool success = true;

            if (!success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return NoContent();
        }
    }
}
