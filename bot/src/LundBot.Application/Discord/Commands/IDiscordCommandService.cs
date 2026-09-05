namespace LundBot.Application.Discord.Commands
{
    public interface IDiscordCommandService
    {
        Task<bool> RefreshCommandsAsync();
        Task<bool> DeleteGlobalApplicationCommandAsync(ulong commandId);
        Task<bool> DeleteGuildApplicationCommandAsync(ulong guildId, ulong commandId);
        Task<bool> DeleteAllGlobalApplicationCommandsAsync();
        Task<bool> DeleteAllGuildApplicationCommandsAsync(ulong guildId);
    }
}
