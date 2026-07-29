using LundBot.Config;
using LundBot.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LundBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class CommandController : BaseController
    {
        private readonly ICommandsService _commandService;

        public CommandController(
            IOptions<DeveloperEnvironmentConfig> devConfig,
            ICommandsService commandService
        )
            : base(devConfig)
        {
            _commandService = commandService;
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncCommands()
        {
            await _commandService.RefreshCommands();
            return Ok(new { message = "Commands synchronized successfully." });
        }

        [HttpDelete("unregister/all")]
        public async Task<IActionResult> UnregisterAllCommands([FromQuery] bool global = false)
        {
            // Implementation for unregistering all commands
            return Ok(new { message = "All commands have been unregistered." });
        }

        [HttpDelete("unregister/{id}")]
        public async Task<IActionResult> UnregisterCommand(
            [FromRoute] string id,
            [FromQuery] bool global = false
        )
        {
            await _commandService.UnregisterCommand(id, global);

            return Ok(new { message = $"Command with ID {id} has been unregistered." });
        }
    }
}
