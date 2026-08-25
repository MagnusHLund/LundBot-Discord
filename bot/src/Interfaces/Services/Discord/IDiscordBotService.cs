using DSharpPlus.Entities;

namespace LundBot.Interfaces.Services.Discord
{
    public interface IDiscordBotService
    {
        Task ConnectBotAsync();
        Task UpdateBotStatusAsync(DiscordActivity activity);
    }
}
