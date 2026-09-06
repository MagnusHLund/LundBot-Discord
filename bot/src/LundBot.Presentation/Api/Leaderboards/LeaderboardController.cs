using LundBot.Application.Features.Leaderboards;
using LundBot.Presentation.Api.Common;
using LundBot.Presentation.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LundBot.Presentation.Api.Leaderboards
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class LeaderboardController : AbstractBaseController
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
        public async Task<IActionResult> RefreshLeaderboard([FromQuery] ulong channelId, [FromQuery] ulong guildId)
        {
            if (!HasApiKey())
            {
                return Unauthorized();
            }

            bool success = await _leaderboardService.RefreshLeaderboardAsync(channelId, guildId);

            if (!success)
            {
                return StatusCode(500, "Failed to refresh leaderboard.");
            }

            return Ok("Leaderboard refreshed successfully.");
        }
    }
}
