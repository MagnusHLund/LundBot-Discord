using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace LundBot.Interfaces.Services.Discord
{
    public interface IDiscordBotService
    {
        Task ConnectBotAsync();
        Task UpdateBotStatusAsync(DiscordActivity activity);
        Task<SlashCommandsExtension> EnableSlashCommands(IServiceProvider services);
    }
}
