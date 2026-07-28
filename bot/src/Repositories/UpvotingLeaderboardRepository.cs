using LundBot.Data;
using LundBot.Entities;
using LundBot.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LundBot.Repositories
{
    public class UpvotingLeaderboardRepository : IUpvotingLeaderboardRepository
    {
        private readonly LundBotDiscordDbContext _dbContext;
        private readonly Serilog.ILogger _logger =
            Serilog.Log.ForContext<UpvotingLeaderboardRepository>();

        public UpvotingLeaderboardRepository(LundBotDiscordDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> HasUserUpvotedTargetAsync(
            string userId,
            string targetUserId,
            int leaderboardId
        )
        {
            try
            {
                return await _dbContext.UpvotingLeaderBoards.AnyAsync(u =>
                    u.DiscordUserIdVoter == userId
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

        public async Task AddUpvoteAsync(string userId, string targetUserId, int leaderboardId)
        {
            try
            {
                var upvote = new UpvotingLeaderBoardEntity
                {
                    DiscordUserIdVoter = userId,
                    DiscordUserIdTarget = targetUserId,
                    LeaderboardsId = leaderboardId,
                };

                _dbContext.UpvotingLeaderBoards.Add(upvote);
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
