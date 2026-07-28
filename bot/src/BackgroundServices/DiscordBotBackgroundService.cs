using DSharpPlus;
using LundBot.Interfaces.Services;

namespace LundBot.BackgroundServices
{
    public sealed class DiscordBotBackgroundService : BackgroundService
    {
        private readonly IBotService _botService;
        private readonly DiscordClient _discordClient;

        public DiscordBotBackgroundService(IBotService botService, DiscordClient discordClient)
        {
            _botService = botService;
            _discordClient = discordClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _botService.InitializeAsync(_discordClient);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
