using DSharpPlus;

namespace LundBot.Interfaces.Services
{
    public interface IBotService
    {
        static DiscordClient DiscordClient { get; set; } = null!;
        Task InitializeAsync(DiscordClient discordClient);
    }
}
