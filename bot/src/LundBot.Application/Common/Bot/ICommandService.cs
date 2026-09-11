namespace LundBot.Application.Common.Bot
{
    public interface ICommandService
    {
        Task<bool> RefreshCommandsAsync();
        Task<bool> UnregisterCommand(ulong commandId, ulong? guildId = null);
        Task<bool> UnregisterAllCommands(ulong? guildId = null);
    }
}
