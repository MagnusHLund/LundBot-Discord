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
        Task UpvoteUserOnLeaderboardAsync(
            DiscordChannel channel,
            DiscordUser userUpvoting,
            DiscordUser userTarget
        );
        Task RegisterUserJoinedWithInviteAsync(
            DiscordGuild guild,
            DiscordUser userJoined,
            DiscordUser userInvitedBy
        );
        Task RegisterWarningOnLeaderboardAsync(DiscordChannel channel, DiscordUser userTarget);
        Task RefreshLeaderboardAsync(ulong channelId, ulong guildId);
        ValueTask<List<LeaderboardsEntity>> GetLeaderboardsForGuildAsync(string guildId);
        Task UpdateLeaderboardMessageAsync(LeaderboardsEntity leaderboard, DiscordChannel channel);
    }
}
