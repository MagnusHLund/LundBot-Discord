using DSharpPlus;

namespace LundBot.Interfaces.Services
{
    public interface IBotService
    {
        Task InitializeAsync(DiscordClient discordClient);
    }
}
