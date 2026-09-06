using LundBot.Application.Common.Bot;
using LundBot.Presentation.Api.Common;
using LundBot.Presentation.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LundBot.Presentation.Api.Bot
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class CommandController : AbstractBaseController
    {
        private readonly ICommandService _commandService;

        public CommandController(
            IOptions<DeveloperEnvironmentConfig> devConfig,
            IOptions<ServerConfig> serverConfig,
            ICommandService commandService
        )
            : base(devConfig, serverConfig)
        {
            _commandService = commandService;
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncCommands()
        {
            // TODO: Add better authentication implementation for all endpoints
            if (!HasApiKey())
            {
                return Unauthorized();
            }

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

        [HttpDelete("unregister/all")]
        public async Task<IActionResult> UnregisterAllCommands([FromQuery] bool global = false)
        {
            if (!HasApiKey())
            {
                return Unauthorized();
            }

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

        [HttpDelete("unregister/{id}")]
        public async Task<IActionResult> UnregisterCommand([FromRoute] string id, [FromQuery] bool global = false)
        {
            if (!HasApiKey())
            {
                return Unauthorized();
            }

            bool success = await _commandService.UnregisterCommand(id, global);

            if (!success)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = $"Failed to unregister command with ID {id}." }
                );
            }

            return Ok(new { message = $"Command with ID {id} has been unregistered." });
        }
    }
}
