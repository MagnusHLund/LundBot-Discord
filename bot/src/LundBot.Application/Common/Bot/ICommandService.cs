namespace LundBot.Application.Common.Bot
{
    public interface ICommandService
    {
        Task LogRegisteredCommandsForGuildsAsync();
        Task<bool> RefreshCommandsAsync();
        Task<bool> UnregisterCommand(string commandId, bool global = false);
        Task<bool> UnregisterAllCommands(bool global = false);
    }
}
