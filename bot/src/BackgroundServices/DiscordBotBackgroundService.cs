using DSharpPlus;
using DSharpPlus.SlashCommands;
using LundBot.Interfaces.Services;
using Serilog;
using ILogger = Serilog.ILogger;

namespace LundBot.BackgroundServices
{
    public sealed class DiscordBotBackgroundService : BackgroundService
    {
        private readonly DiscordClient _discordClient;
        private readonly IBotService _botService;
        private readonly ICommandsService _commandsService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _logger = Log.ForContext<DiscordBotBackgroundService>();

        public DiscordBotBackgroundService(
            DiscordClient discordClient,
            IBotService botService,
            ICommandsService commandsService,
            IServiceScopeFactory scopeFactory
        )
        {
            _discordClient = discordClient;
            _botService = botService;
            _commandsService = commandsService;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _discordClient.Ready += async (sender, e) =>
            {
                _logger.Information("Discord client is ready. Starting bot initialization...");

                await _botService.InitializeAsync(_discordClient);

                _logger.Information("Bot initialization completed.");
            };

            try
            {
                _logger.Information("Enabling slash commands dependency...");
                var slash = _discordClient.UseSlashCommands();

                _logger.Information("Connecting to Discord...");
                await _discordClient.ConnectAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error initializing Discord client. Exception: {ExceptionMessage}",
                    ex.Message
                );
            }

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
