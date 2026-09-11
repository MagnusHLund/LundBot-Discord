using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Presentation.Api.Common;
using LundBot.Presentation.Api.Common.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LundBot.Presentation.Api.Leaderboards
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class LeaderboardController : AbstractBaseController
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshLeaderboard(
            [FromQuery, DiscordId] ulong channelId,
            [FromQuery, DiscordId] ulong guildId
        )
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
