using DSharpPlus;

namespace LundBot.Interfaces.Services
{
    public interface ICommandsService
    {
        Task RegisterCommandsAsync();
        Task LogRegisteredCommandsForGuildsAsync();
        Task RefreshCommands();
        Task UnregisterCommand(string commandId, bool global = false);
        Task UnregisterAllCommands(bool global = false);
    }
}
