using LundBot.Application.Discord.Channels;
using LundBot.Domain.Leaderboards;

namespace LundBot.Application.Features.Leaderboards.Contracts
{
    public interface IAbstractLeaderboardService
    {
        Task<bool> CreateLeaderboardAsync(
            ulong channelId,
            string title,
            string message,
            LeaderboardTypeEnum leaderboardType
        );
        Task<bool> RemoveLeaderboardAsync(ulong channelId);
        Task<bool> RefreshLeaderboardAsync(ulong channelId, ulong guildId);
        Task<bool> UpdateLeaderboardMessageAsync(Leaderboard leaderboard, DiscordChannelDto channel);
        ValueTask<List<Leaderboard>> GetLeaderboardsForGuildAsync(ulong guildId);
    }
}
