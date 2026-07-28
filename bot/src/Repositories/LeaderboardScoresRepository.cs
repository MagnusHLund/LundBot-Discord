using LundBot.Data;
using LundBot.Entities;
using LundBot.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LundBot.Repositories
{
    public class LeaderboardScoresRepository : ILeaderboardScoresRepository
    {
        private readonly LundBotDiscordDbContext _dbContext;
        private readonly Serilog.ILogger _logger =
            Serilog.Log.ForContext<LeaderboardScoresRepository>();

        public LeaderboardScoresRepository(LundBotDiscordDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task IncrementScoreAsync(string userId, int leaderboardId)
        {
            try
            {
                var leaderboardScore = await _dbContext.LeaderboardScores.FirstOrDefaultAsync(ls =>
                    ls.DiscordUserId == userId && ls.LeaderboardsId == leaderboardId
                );

                if (leaderboardScore != null)
                {
                    leaderboardScore.Score += 1;
                }
                else
                {
                    leaderboardScore = new LeaderboardScoresEntity
                    {
                        DiscordUserId = userId,
                        LeaderboardsId = leaderboardId,
                        Score = 1,
                    };
                    _dbContext.LeaderboardScores.Add(leaderboardScore);
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while incrementing the score.");
                throw new Exception("An error occurred while incrementing the score.", ex);
            }
        }

        public async Task<IEnumerable<LeaderboardScoresEntity>> GetTopScoresAsync(
            int leaderboardId,
            int limit
        )
        {
            try
            {
                return await _dbContext
                    .LeaderboardScores.Where(ls => ls.LeaderboardsId == leaderboardId)
                    .OrderByDescending(ls => ls.Score)
                    .ThenBy(ls => ls.UpdatedAt)
                    .Take(limit)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while retrieving top scores.");
                throw new Exception("An error occurred while retrieving top scores.", ex);
            }
        }
    }
}
