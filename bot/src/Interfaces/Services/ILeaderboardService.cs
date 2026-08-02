using DSharpPlus.Entities;
using LundBot.Entities;
using LundBot.Enums;

namespace LundBot.Interfaces.Services
{
    public interface ILeaderboardService
    {
        Task CreateLeaderboardAsync(
            DiscordChannel channel,
            string title,
            string message,
            LeaderboardType leaderboardType
        );
        Task RemoveLeaderboardAsync(DiscordChannel channel);
        Task UpvoteUserOnLeaderboard(
            DiscordChannel channel,
            DiscordUser userUpvoting,
            DiscordUser userTarget
        );
        Task RegisterUserJoinedWithInvite(
            DiscordGuild guild,
            DiscordUser userJoined,
            DiscordUser userInvitedBy
        );
        Task RefreshLeaderboardAsync(ulong channelId, ulong guildId);
        ValueTask<List<LeaderboardsEntity>> GetLeaderboardsForGuildAsync(string guildId);
    }
}
