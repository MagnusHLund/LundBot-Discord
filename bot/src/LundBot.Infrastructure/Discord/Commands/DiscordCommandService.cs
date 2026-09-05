using DSharpPlus.Commands;
using LundBot.Application.Discord.Commands;
using Serilog;

namespace LundBot.Infrastructure.Discord.Commands
{
    public sealed class DiscordCommandService : IDiscordCommandService
    {
        private readonly CommandsExtension _commands;

        private readonly ILogger _logger = Log.ForContext<DiscordCommandService>();

        public DiscordCommandService(CommandsExtension commands)
        {
            _commands = commands;
        }

        public async Task RefreshCommandsAsync()
        {
            _logger.Information("Refreshing commands...");

            try
            {
                await _commands.RefreshAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to refresh commands");
            }
        }
    }
}
