using DSharpPlus.Entities;
using LundBot.Interfaces.Services.Discord;
using Serilog;

namespace LundBot.Services.Discord
{
    public sealed class DiscordBotService : IDiscordBotService
    {
        private readonly Serilog.ILogger _logger = Log.ForContext<DiscordBotService>();

        public async Task ConnectBotAsync()
        {
            _logger.Information("Connecting to Discord...");

            try
            {
                await BotService.DiscordClient.ConnectAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to connect to Discord.");
            }
        }

        public async Task UpdateBotStatusAsync(DiscordActivity activity)
        {
            _logger.Information("Updating bot status...");

            try
            {
                await BotService.DiscordClient.UpdateStatusAsync(
                    activity,
                    DiscordUserStatus.Online
                );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update bot status.");
            }
        }
    }
}
