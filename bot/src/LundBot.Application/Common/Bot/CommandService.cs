using LundBot.Application.Discord.Commands;

namespace LundBot.Application.Common.Bot
{
    public sealed class CommandService : ICommandService
    {
        private readonly IDiscordCommandService _discordCommandService;

        private readonly ILogger _logger = Log.ForContext<CommandService>();

        public CommandService(IDiscordCommandService discordCommandService)
        {
            _discordCommandService = discordCommandService;
        }

        public async Task<bool> RefreshCommandsAsync()
        {
            return await _discordCommandService.RefreshCommandsAsync();
        }

        public async Task<bool> UnregisterAllCommands(ulong? guildId = null)
        {
            if (guildId is null)
            {
                return await _discordCommandService.DeleteAllGlobalApplicationCommandsAsync();
            }

            return await _discordCommandService.DeleteAllGuildApplicationCommandsAsync(guildId.Value);
        }

        public async Task<bool> UnregisterCommand(ulong commandId, ulong? guildId = null)
        {
            if (guildId is null)
            {
                return await _discordCommandService.DeleteGlobalApplicationCommandAsync(commandId);
            }

            return await _discordCommandService.DeleteGuildApplicationCommandAsync(guildId.Value, commandId);
        }
    }
}
