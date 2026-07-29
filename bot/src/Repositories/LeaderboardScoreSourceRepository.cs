using LundBot.Data;
using LundBot.Entities;
using LundBot.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LundBot.Repositories
{
    public class LeaderboardScoreSourceRepository : ILeaderboardScoreSourceRepository
    {
        private readonly LundBotDiscordDbContext _dbContext;
        private readonly Serilog.ILogger _logger =
            Serilog.Log.ForContext<LeaderboardScoreSourceRepository>();

        public LeaderboardScoreSourceRepository(LundBotDiscordDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> HasUserGivenScoreToTargetAsync(
            string userId,
            string targetUserId,
            int leaderboardId
        )
        {
            try
            {
                return await _dbContext.LeaderboardScoreSources.AnyAsync(u =>
                    u.DiscordUserIdActor == userId
                    && u.DiscordUserIdTarget == targetUserId
                    && u.LeaderboardsId == leaderboardId
                );
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error checking if user {UserId} has upvoted target {TargetUserId} on leaderboard {LeaderboardId}",
                    userId,
                    targetUserId,
                    leaderboardId
                );
                throw new Exception("An error occurred while checking upvote status.", ex);
            }
        }

        public async Task AddScoreAsync(string userId, string targetUserId, int leaderboardId)
        {
            try
            {
                var upvote = new LeaderboardScoreSourceEntity
                {
                    DiscordUserIdActor = userId,
                    DiscordUserIdTarget = targetUserId,
                    LeaderboardsId = leaderboardId,
                };

                _dbContext.LeaderboardScoreSources.Add(upvote);
                await _dbContext.SaveChangesAsync();
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
                throw new Exception("An error occurred while adding the upvote.", ex);
            }
        }
    }
}
