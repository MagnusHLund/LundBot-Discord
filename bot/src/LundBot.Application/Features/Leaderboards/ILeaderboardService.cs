using LundBot.Application.Discord.Channels;
using LundBot.Application.Discord.Guilds;
using LundBot.Application.Discord.Users;
using LundBot.Domain.Leaderboards;

namespace LundBot.Application.Features.Leaderboards
{
    public interface ILeaderboardService
    {
        Task<bool> CreateLeaderboardAsync(
            ulong channelId,
            string title,
            string message,
            LeaderboardTypeEnum leaderboardType
        );
        Task<bool> RemoveLeaderboardAsync(ulong channelId);
        Task<bool> UpvoteUserOnLeaderboardAsync(
            ulong channelId,
            DiscordUserDto userUpvoting,
            DiscordUserDto targetUser
        );
        Task<bool> RegisterWarningOnLeaderboardAsync(ulong channelId, DiscordUserDto targetUser);
        Task RegisterUserJoinedWithInviteAsync(
            DiscordGuildDto guild,
            DiscordUserDto userJoined,
            DiscordUserDto userInvitedBy
        );
        Task<bool> RefreshLeaderboardAsync(ulong channelId, ulong guildId);
        ValueTask<List<Leaderboard>> GetLeaderboardsForGuildAsync(ulong guildId);
        Task UpdateLeaderboardMessageAsync(Leaderboard leaderboard, DiscordChannelDto channel);
    }
}
