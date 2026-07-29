using DSharpPlus;

namespace LundBot.Interfaces.Services
{
    public interface ICommandsService
    {
        Task RegisterCommandsAsync();
        Task LogRegisteredCommandsForGuildsAsync();
        Task RefreshCommands();
        Task<bool> UnregisterCommand(string commandId, bool global = false);
        Task<bool> UnregisterAllCommands(bool global = false);
    }
}
