using LundBot.Application.Discord.Commands;

namespace LundBot.Application.Common.Bot
{
    public sealed class CommandService : ICommandService
    {
        private readonly IDiscordCommandService _discordCommandService;

        public CommandService(IDiscordCommandService discordCommandService)
        {
            _discordCommandService = discordCommandService;
        }

        public Task LogRegisteredCommandsForGuildsAsync()
        {
            return Task.CompletedTask;
        }

        public async Task<bool> RefreshCommandsAsync()
        {
            return await _discordCommandService.RefreshCommandsAsync();
        }

        public Task<bool> RegisterCommandsAsync()
        {
            return Task.FromResult(true);
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
