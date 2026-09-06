namespace LundBot.Application.Common.Bot
{
    public sealed class CommandService : ICommandService
    {
        public Task LogRegisteredCommandsForGuildsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> RefreshCommandsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> RegisterCommandsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UnregisterAllCommands(bool global = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UnregisterCommand(string commandId, bool global = false)
        {
            throw new NotImplementedException();
        }
    }
}
