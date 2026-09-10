using LundBot.Application.Discord.Users;

namespace LundBot.Application.Features.Leaderboards.Contracts
{
    public interface IUpvoteLeaderboardService : IAbstractLeaderboardService
    {
        Task<bool> UpvoteUserAsync(ulong channelId, DiscordUserDto userUpvoting, DiscordUserDto targetUser);
    }
}
