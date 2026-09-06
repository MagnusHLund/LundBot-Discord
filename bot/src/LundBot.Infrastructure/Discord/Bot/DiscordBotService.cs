using DSharpPlus;
using DSharpPlus.Entities;
using LundBot.Application.Discord.Bot;

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

        public async Task<bool> ConnectToDiscordAsync()
        {
            _logger.Information("Connecting to Discord...");

            try
            {
                await _discordClient.ConnectAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to connect to Discord.");
                return false;
            }
        }

        public async Task<bool> UpdateBotStatusAsync(string message)
        {
            _logger.Information("Updating bot status...");

            DiscordActivity activity = new DiscordActivity(message, DiscordActivityType.Playing);

            try
            {
                await _discordClient.UpdateStatusAsync(activity);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update bot status.");
                return false;
            }
        }
    }
}
