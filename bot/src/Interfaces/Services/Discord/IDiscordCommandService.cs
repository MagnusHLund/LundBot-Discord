using DSharpPlus.Commands;
using DSharpPlus.Entities;

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
        Task<CommandsExtension> GetSlashCommandsAsync();
        Task RefreshCommandsAsync(CommandsExtension slashCommands);
    }
}
