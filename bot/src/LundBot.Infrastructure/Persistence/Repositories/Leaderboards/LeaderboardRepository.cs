
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Domain.Leaderboards;

namespace LundBot.Infrastructure.Persistence.Repositories.Leaderboards
{
    public sealed class LeaderboardRepository : ILeaderboardRepository
    {
        public Task<(bool, Leaderboard?)> DoesLeaderboardExistAsync(ulong channelId, ulong guildId)
        {
            return Task.FromResult((false, (Leaderboard?)null));
        }

        public Task<Leaderboard> CreateLeaderboardAsync(
            ulong channelId,
            ulong guildId,
            string title,
            string message,
            LeaderboardTypeEnum leaderboardType
        )
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveLeaderboardAsync(ulong channelId, ulong guildId)
        {
            throw new NotImplementedException();
        }

        public Task<(bool, Leaderboard?)> DoesInviteLeaderboardExistOnServerAsync(ulong guildId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Leaderboard>> GetLeaderboardsForGuildAsync(ulong guildId)
        {
            throw new NotImplementedException();
        }
    }
}
