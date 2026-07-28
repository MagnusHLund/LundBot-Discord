using DSharpPlus.Entities;

namespace LundBot.Interfaces.Services
{
    public interface ILeaderboardService
    {
        Task CreateUpvoteLeaderboardAsync(DiscordChannel channel, string title, string message);
        Task CreateInviteLeaderboardAsync(DiscordChannel channel, string title, string message);
        Task RemoveLeaderboardAsync(DiscordChannel channel);
        Task UpvoteUserOnLeaderboard(
            DiscordChannel channel,
            DiscordUser userUpvoting,
            DiscordUser userTarget
        );
    }
}
