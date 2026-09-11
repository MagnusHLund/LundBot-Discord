using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Presentation.Api.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LundBot.Presentation.Api.Leaderboards
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class LeaderboardController : AbstractBaseController
    {
        private readonly IAbstractLeaderboardService _leaderboardService;

        public LeaderboardController(IAbstractLeaderboardService leaderboardService)
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
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "Failed to refresh leaderboard." }
                );
            }

            return NoContent();
        }
    }
}
