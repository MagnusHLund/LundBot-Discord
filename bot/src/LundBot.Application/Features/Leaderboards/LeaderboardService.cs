using LundBot.Application.Discord.Channels;
using LundBot.Application.Discord.Users;
using LundBot.Domain.Leaderboards;

namespace LundBot.Application.Features.Leaderboards
{
    public sealed class LeaderboardService : ILeaderboardService
    {
        public Task CreateLeaderboardAsync(
            ulong channelId,
            string title,
            string message,
            LeaderboardTypeEnum leaderboardType
        )
        {
            throw new NotImplementedException();
        }

        public ValueTask<List<Leaderboard>> GetLeaderboardsForGuildAsync(ulong guildId)
        {
            throw new NotImplementedException();
        }

        public Task RefreshLeaderboardAsync(ulong channelId, ulong guildId)
        {
            throw new NotImplementedException();
        }

        public Task RegisterUserJoinedWithInviteAsync(
            ulong guildId,
            DiscordUserDto userJoined,
            DiscordUserDto invitedByUser
        )
        {
            throw new NotImplementedException();
        }

        public Task RegisterWarningOnLeaderboardAsync(ulong channelId, DiscordUserDto targetUser)
        {
            throw new NotImplementedException();
        }

        public Task RemoveLeaderboardAsync(ulong channelId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateLeaderboardMessageAsync(Leaderboard leaderboard, DiscordChannelDto channel)
        {
            throw new NotImplementedException();
        }

        public Task UpvoteUserOnLeaderboardAsync(
            ulong channelId,
            DiscordUserDto userUpvoting,
            DiscordUserDto targetUser
        )
        {
            throw new NotImplementedException();
        }
    }
}
