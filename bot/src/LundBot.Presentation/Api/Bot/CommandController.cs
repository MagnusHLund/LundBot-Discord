using LundBot.Application.Common.Bot;
using LundBot.Presentation.Api.Common;
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

            return Ok(new { message = "Commands synchronized successfully." });
        }

        [Authorize]
        [HttpDelete("unregister/all")]
        public async Task<IActionResult> UnregisterAllCommands([FromQuery] bool global = false)
        {
            bool success = await _commandService.UnregisterAllCommands(global);

            if (!success)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "Failed to unregister all commands." }
                );
            }

            return Ok(new { message = "All commands have been unregistered." });
        }

        [Authorize]
        [HttpDelete("unregister/{id}")]
        public async Task<IActionResult> UnregisterCommand([FromRoute] ulong commandId, [FromQuery] bool global = false)
        {
            bool success = await _commandService.UnregisterCommand(commandId, global);

            if (!success)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = $"Failed to unregister command with ID {commandId}." }
                );
            }

            return Ok(new { message = $"Command with ID {commandId} has been unregistered." });
        }
    }
}
