using DSharpPlus;

namespace LundBot.Interfaces.Services
{
    public interface ICommandsService
    {
        Task RegisterCommandsAsync();
        Task LogRegisteredCommandsForGuildsAsync();
        Task RefreshCommands();
    }
}
