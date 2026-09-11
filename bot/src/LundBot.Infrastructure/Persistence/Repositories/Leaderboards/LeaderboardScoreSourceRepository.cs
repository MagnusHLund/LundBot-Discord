using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Domain.Leaderboards;
using Microsoft.EntityFrameworkCore;

namespace LundBot.Infrastructure.Persistence.Repositories.Leaderboards
{
    public sealed class LeaderboardScoreSourceRepository : ILeaderboardScoreSourceRepository
    {
        private readonly LundBotDbContext _context;
        private readonly ILogger _logger = Log.ForContext<LeaderboardScoreSourceRepository>();

        public LeaderboardScoreSourceRepository(LundBotDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasUserGivenScoreToTargetAsync(ulong userId, ulong targetUserId, int leaderboardId)
        {
            try
            {
                return await _context.LeaderboardScoreSources.AnyAsync(u =>
                    u.DiscordUserIdActor == userId
                    && u.DiscordUserIdTarget == targetUserId
                    && u.LeaderboardId == leaderboardId
                );
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error checking if user {UserId} has given score to target {TargetUserId} on leaderboard {LeaderboardId}",
                    userId,
                    targetUserId,
                    leaderboardId
                );
                throw new Exception("An error occurred while checking score status.", ex);
            }
        }

        public async Task<bool> AddScoreAsync(ulong userId, ulong targetUserId, int leaderboardId)
        {
            try
            {
                LeaderboardScoreSource upvote = new LeaderboardScoreSource
                {
                    DiscordUserIdActor = userId,
                    DiscordUserIdTarget = targetUserId,
                    LeaderboardId = leaderboardId,
                };

                _context.LeaderboardScoreSources.Add(upvote);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error adding upvote from user {UserId} to target {TargetUserId} on leaderboard {LeaderboardId}",
                    userId,
                    targetUserId,
                    leaderboardId
                );
                return false;
            }
        }
    }
}
