using LundBot.Application.Common.Exceptions;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Domain.Leaderboards;
using LundBot.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace LundBot.Infrastructure.Persistence.Repositories.Leaderboards
{
    public sealed class LeaderboardScoreRepository : ILeaderboardScoreRepository
    {
        private readonly LundBotDbContext _context;
        private readonly ILogger _logger = Log.ForContext<LeaderboardScoreRepository>();

        public LeaderboardScoreRepository(LundBotDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IncrementScoreAsync(ulong userId, int leaderboardId)
        {
            try
            {
                int rowsAffected = await _context
                    .LeaderboardScores.Where(ls => ls.DiscordUserId == userId && ls.LeaderboardId == leaderboardId)
                    .ExecuteUpdateAsync(setters =>
                        setters
                            .SetProperty(ls => ls.Score, ls => ls.Score + 1)
                            .SetProperty(ls => ls.UpdatedAt, _ => DateTime.UtcNow)
                    );

                if (rowsAffected > 0)
                {
                    return true;
                }

                return await CreateInitialScoreAsync(userId, leaderboardId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while incrementing the score.");
                return false;
            }
        }

        private async Task<bool> CreateInitialScoreAsync(ulong userId, int leaderboardId)
        {
            LeaderboardScore leaderboardScore = new LeaderboardScore
            {
                DiscordUserId = userId,
                LeaderboardId = leaderboardId,
                Score = 1,
            };

            try
            {
                _context.LeaderboardScores.Add(leaderboardScore);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex) when (DatabaseUtils.IsUniqueConstraintViolation(ex))
            {
                _context.Entry(leaderboardScore).State = EntityState.Detached;

                int rowsAffected = await _context
                    .LeaderboardScores.Where(ls => ls.DiscordUserId == userId && ls.LeaderboardId == leaderboardId)
                    .ExecuteUpdateAsync(setters =>
                        setters
                            .SetProperty(ls => ls.Score, ls => ls.Score + 1)
                            .SetProperty(ls => ls.UpdatedAt, _ => DateTime.UtcNow)
                    );

                return rowsAffected > 0;
            }
        }

        public async Task<IEnumerable<LeaderboardScore>> GetTopScoresAsync(int leaderboardId, int limit)
        {
            try
            {
                return await _context
                    .LeaderboardScores.Where(ls => ls.LeaderboardId == leaderboardId)
                    .OrderByDescending(ls => ls.Score)
                    .ThenBy(ls => ls.UpdatedAt)
                    .Take(limit)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while retrieving top scores.");

                throw new RepositoryException($"Failed to retrieve top scores for leaderboard ID {leaderboardId}.", ex);
            }
        }
    }
}
