namespace LundBot.Application.Common.Bot
{
    public interface ICommandService
    {
        Task<bool> RefreshCommandsAsync();
        Task<bool> UnregisterCommand(ulong commandId, bool global = false, ulong? guildId = null);
        Task<bool> UnregisterAllCommands(bool global = false, ulong? guildId = null);
    }
}
