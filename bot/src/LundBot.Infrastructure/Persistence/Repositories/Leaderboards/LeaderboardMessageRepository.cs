using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Domain.Leaderboards;

namespace LundBot.Infrastructure.Persistence.Repositories.Leaderboards
{
    public sealed class LeaderboardMessageRepository : ILeaderboardMessageRepository
    {
        public Task<bool> CreateAsync(LeaderboardMessage entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<LeaderboardMessage>> GetMessagesForLeaderboardAsync(int leaderboardId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(LeaderboardMessage entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteManyAsync(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }
    }
}
