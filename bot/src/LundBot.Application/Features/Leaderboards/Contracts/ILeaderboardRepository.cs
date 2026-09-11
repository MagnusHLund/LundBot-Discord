using LundBot.Domain.Leaderboards;

namespace LundBot.Application.Features.Leaderboards.Contracts
{
    public interface ILeaderboardRepository
    {
        Task<(bool, Leaderboard?)> DoesLeaderboardExistAsync(ulong channelId, ulong guildId);
        Task<Leaderboard> CreateLeaderboardAsync(
            ulong channelId,
            ulong guildId,
            string title,
            string message,
            LeaderboardTypeEnum leaderboardType
        );
        Task<bool> RemoveLeaderboardAsync(ulong channelId, ulong guildId);
        Task<(bool, Leaderboard?)> DoesInviteLeaderboardExistOnServerAsync(ulong guildId);
        Task<List<Leaderboard>> GetLeaderboardsForGuildAsync(ulong guildId);
    }
}
