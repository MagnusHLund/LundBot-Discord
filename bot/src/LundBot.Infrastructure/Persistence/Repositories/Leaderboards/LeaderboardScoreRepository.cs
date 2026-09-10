using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Domain.Leaderboards;

namespace LundBot.Infrastructure.Persistence.Repositories.Leaderboards
{
    public sealed class LeaderboardScoreRepository : ILeaderboardScoreRepository
    {
        public Task<bool> IncrementScoreAsync(ulong userId, int leaderboardId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LeaderboardScore>> GetTopScoresAsync(int leaderboardId, int limit)
        {
            throw new NotImplementedException();
        }
    }
}
