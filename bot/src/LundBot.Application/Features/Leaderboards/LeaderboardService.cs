using LundBot.Application.Discord.Channels;
using LundBot.Application.Discord.Users;
using LundBot.Domain.Leaderboards;

namespace LundBot.Application.Features.Leaderboards
{
    public sealed class LeaderboardService : ILeaderboardService
    {
        public Task<bool> CreateLeaderboardAsync(
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

        public Task<bool> RefreshLeaderboardAsync(ulong channelId, ulong guildId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RegisterWarningOnLeaderboardAsync(ulong channelId, DiscordUserDto targetUser)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveLeaderboardAsync(ulong channelId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateLeaderboardMessageAsync(Leaderboard leaderboard, DiscordChannelDto channel)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpvoteUserOnLeaderboardAsync(
            ulong channelId,
            DiscordUserDto userUpvoting,
            DiscordUserDto targetUser
        )
        {
            throw new NotImplementedException();
        }
    }
}
