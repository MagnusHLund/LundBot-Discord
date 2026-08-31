using LundBot.Config;
using LundBot.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LundBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaderboardController : BaseController
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardController(
            IOptions<DeveloperEnvironmentConfig> devConfig,
            IOptions<ServerConfig> serverConfig,
            ILeaderboardService leaderboardService
        )
            : base(devConfig, serverConfig)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshLeaderboard(
            [FromQuery] ulong channelId,
            [FromQuery] ulong guildId
        )
        {
            if (!HasApiKey())
            {
                return Unauthorized();
            }

            try
            {
                await _leaderboardService.RefreshLeaderboardAsync(channelId, guildId);
                return Ok("Leaderboard refreshed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error refreshing leaderboard: {ex.Message}");
            }
        }
    }
}
