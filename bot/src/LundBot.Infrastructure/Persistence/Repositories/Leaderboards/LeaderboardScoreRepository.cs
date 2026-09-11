using LundBot.Application.Common.Exceptions;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Domain.Leaderboards;
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
                var leaderboardScore = await _context.LeaderboardScores.FirstOrDefaultAsync(ls =>
                    ls.DiscordUserId == userId && ls.LeaderboardId == leaderboardId
                );

                if (leaderboardScore != null)
                {
                    leaderboardScore.Score += 1;
                }
                else
                {
                    leaderboardScore = new LeaderboardScore
                    {
                        DiscordUserId = userId,
                        LeaderboardId = leaderboardId,
                        Score = 1,
                    };
                    _context.LeaderboardScores.Add(leaderboardScore);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while incrementing the score.");
                return false;
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

                throw new RepositoryException(
                    $"Failed to retrieve top scores for leaderboard ID {leaderboardId}.",
                    ex
                );
            }
        }
    }
}
