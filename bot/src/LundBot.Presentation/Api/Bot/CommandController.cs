using LundBot.Application.Common.Bot;
using LundBot.Presentation.Api.Common;
using LundBot.Presentation.Api.Common.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LundBot.Presentation.Api.Bot
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class CommandController : AbstractBaseController
    {
        private readonly ICommandService _commandService;

        public CommandController(ICommandService commandService)
        {
            _commandService = commandService;
        }

        [Authorize]
        [HttpPost("sync")]
        public async Task<IActionResult> SyncCommands()
        {
            bool success = await _commandService.RefreshCommandsAsync();

            if (!success)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "Failed to synchronize commands." }
                );
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("unregister/all")]
        public async Task<IActionResult> UnregisterAllCommands([FromQuery, DiscordId] ulong? guildId = null)
        {
            bool success = await _commandService.UnregisterAllCommands(guildId);

            if (!success)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "Failed to unregister all commands." }
                );
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("unregister/{commandId}")]
        public async Task<IActionResult> UnregisterCommand(
            [FromRoute, DiscordId] ulong commandId,
            [FromQuery, DiscordId] ulong? guildId = null
        )
        {
            bool success = await _commandService.UnregisterCommand(commandId, guildId);

            if (!success)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = $"Failed to unregister command with ID {commandId}." }
                );
            }

            return NoContent();
        }
    }
}
