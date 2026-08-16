using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace LundBot.Interfaces.Services.Discord
{
    public interface IDiscordCommandService
    {
        Task<IReadOnlyList<DiscordApplicationCommand>> GetGlobalApplicationCommandsAsync();
        Task<IReadOnlyList<DiscordApplicationCommand>> GetGuildApplicationCommandsAsync(
            ulong guildId
        );
        Task DeleteGlobalApplicationCommandAsync(ulong commandId);
        Task DeleteGuildApplicationCommandAsync(ulong guildId, ulong commandId);
        Task<SlashCommandsExtension> GetSlashCommandsAsync();
        Task RefreshCommandsAsync(SlashCommandsExtension slashCommands);
    }
}
