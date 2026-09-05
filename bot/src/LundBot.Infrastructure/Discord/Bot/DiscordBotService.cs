using DSharpPlus;
using LundBot.Application.Discord.Bot;
using Serilog;

namespace LundBot.Infrastructure.Discord.Bot
{
    public sealed class DiscordBotService : IDiscordBotService
    {
        private readonly DiscordClient _discordClient;

        private readonly ILogger _logger = Log.ForContext<DiscordBotService>();

        public DiscordBotService(DiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        public async Task ConnectToDiscordAsync()
        {
            _logger.Information("Connecting to Discord...");

            try
            {
                await _discordClient.ConnectAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to connect to Discord.");
            }
        }

        public async Task UpdateBotStatusAsync()
        {
            _logger.Information("Updating bot status...");

            try
            {
                await _discordClient.UpdateStatusAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update bot status.");
            }
        }
    }
}
