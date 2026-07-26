using DSharpPlus;

namespace LundBot.Interfaces.Services
{
    public interface ICommandsService
    {
        Task RegisterCommandsAsync(DiscordClient discordClient);
        Task LogRegisteredCommandsForGuildsAsync(DiscordClient discordClient);
        Task RefreshCommands(DiscordClient discordClient);
    }
}
