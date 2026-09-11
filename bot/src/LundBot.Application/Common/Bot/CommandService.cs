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

        public async Task<bool> UnregisterAllCommands(bool global = false, ulong? guildId = null)
        {
            if (global)
            {
                return await _discordCommandService.DeleteAllGlobalApplicationCommandsAsync();
            }

            if (guildId is null)
            {
                _logger.Error("Guild ID must be provided for guild-specific command deletion.");
                return false;
            }

            return await _discordCommandService.DeleteAllGuildApplicationCommandsAsync(guildId.Value);
        }

        public async Task<bool> UnregisterCommand(ulong commandId, bool global = false, ulong? guildId = null)
        {
            if (global)
            {
                return await _discordCommandService.DeleteGlobalApplicationCommandAsync(commandId);
            }

            if (guildId is null)
            {
                _logger.Error("Guild ID must be provided for guild-specific command deletion.");
                return false;
            }

            return await _discordCommandService.DeleteGuildApplicationCommandAsync(guildId.Value, commandId);
        }
    }
}
