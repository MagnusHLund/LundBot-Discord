using LundBot.Application.Discord.Channels;
using LundBot.Application.Discord.Users;
using LundBot.Domain.Leaderboards;

namespace LundBot.Application.Features.Leaderboards
{
    public interface ILeaderboardService
    {
        Task CreateLeaderboardAsync(ulong channelId, string title, string message, LeaderboardTypeEnum leaderboardType);
        Task RemoveLeaderboardAsync(ulong channelId);
        Task UpvoteUserOnLeaderboardAsync(ulong channelId, DiscordUserDto userUpvoting, DiscordUserDto targetUser);
        Task RegisterUserJoinedWithInviteAsync(ulong guildId, DiscordUserDto userJoined, DiscordUserDto invitedByUser);
        Task RegisterWarningOnLeaderboardAsync(ulong channelId, DiscordUserDto targetUser);
        Task RefreshLeaderboardAsync(ulong channelId, ulong guildId);
        ValueTask<List<Leaderboard>> GetLeaderboardsForGuildAsync(ulong guildId);
        Task UpdateLeaderboardMessageAsync(Leaderboard leaderboard, DiscordChannelDto channel);
    }
}
