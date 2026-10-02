using LundBot.Application.Features.Moderation;
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
        private readonly IModerationActionService _moderationActionService;

        public ModerationController(IModerationActionService moderationActionService)
        {
            _moderationActionService = moderationActionService;
        }

        [Authorize]
        [HttpPost("kick/roles/assign")]
        public async Task<IActionResult> AssignRoleToAutomaticallyKick([FromBody] AssignRoleToKickRequestDto request)
        {
            bool success = await _moderationActionService.SetRoleToAutomaticallyKickAsync(
                request.GuildId,
                request.RoleId,
                request.KickReason
            );

            if (!success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("kick/roles/unassign")]
        public async Task<IActionResult> UnassignRoleToAutomaticallyKick(
            [FromBody] UnassignRoleToKickRequestDto request
        )
        {
            bool success = await _moderationActionService.RemoveRoleFromAutomaticallyKickAsync(
                request.GuildId,
                request.RoleId
            );

            if (!success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return NoContent();
        }
    }
}
