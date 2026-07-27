using DSharpPlus.Entities;

namespace LundBot.Interfaces.Services
{
    public interface ILeaderboardService
    {
        Task CreateUpvoteLeaderboardAsync(DiscordChannel channel, string title, string message);
    }
}
