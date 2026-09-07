using LundBot.Application.Features.Leaderboards;
using LundBot.Presentation.Api.Common;
using LundBot.Presentation.Config;
using Microsoft.AspNetCore.Authorization;
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
            ILeaderboardService leaderboardService
        )
            : base(devConfig)
        {
            _leaderboardService = leaderboardService;
        }

        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshLeaderboard([FromQuery] ulong channelId, [FromQuery] ulong guildId)
        {
            bool success = await _leaderboardService.RefreshLeaderboardAsync(channelId, guildId);

            if (!success)
            {
                return StatusCode(500, "Failed to refresh leaderboard.");
            }

            return Ok("Leaderboard refreshed successfully.");
        }
    }
}
