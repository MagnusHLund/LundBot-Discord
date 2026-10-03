using System.Reflection;
using DSharpPlus;
using LundBot.Application.Common.Bot;
using LundBot.Application.Discord.Bot;
using LundBot.Presentation.Config;
using LundBot.Presentation.Discord.Commands;
using Microsoft.Extensions.Options;

namespace LundBot.Presentation.Discord.Bot
{
    public sealed class DiscordBotBackgroundService : BackgroundService
    {
        private readonly ServerConfig _serverConfig;
        private readonly ICommandService _commandsService;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IDiscordBotService _discordBotService;
        private readonly IDiscordCommandRegistration _discordCommandRegistration;

        private readonly ILogger _logger = Log.ForContext<DiscordBotBackgroundService>();

        public DiscordBotBackgroundService(
            IOptions<ServerConfig> serverConfig,
            ICommandService commandsService,
            IDiscordBotService discordBotService,
            IDiscordCommandRegistration discordCommandRegistration,
            IHostEnvironment hostEnvironment
        )
        {
            _serverConfig = serverConfig.Value;
            _commandsService = commandsService;
            _discordBotService = discordBotService;
            _discordCommandRegistration = discordCommandRegistration;
            _hostEnvironment = hostEnvironment;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool initialized = await InitializeAsync(stoppingToken);
            if (!initialized)
            {
                return;
            }

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task<bool> InitializeAsync(CancellationToken stoppingToken)
        {
            string dSharpPlusVersion =
                typeof(DiscordClient)
                    .Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion
                ?? "Unknown";

            _logger.Information(
                "Initializing Bot version {Version} in {Environment} mode... (DSharpPlus version: {DSharpPlusVersion})",
                _serverConfig.Version,
                _hostEnvironment.EnvironmentName,
                dSharpPlusVersion
            );

            ushort retries = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                bool successRegisterCommands = await _discordCommandRegistration.RegisterCommandsAsync();
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                bool successConnectToDiscord = await _discordBotService.ConnectToDiscordAsync();

                if (successRegisterCommands && successConnectToDiscord)
                {
                    _logger.Information("Bot initialization is complete!");
                    return true;
                }

                retries++;
                _logger.Warning("Initialization failed. Retrying... Attempt {Retries}", retries);

                try
                {
                    await Task.Delay(GetDelay(retries), stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }

            _logger.Information("Bot initialization was cancelled because the host is shutting down.");
            return false;
        }

        private static int GetDelay(ushort retries)
        {
            return retries switch
            {
                1 => 5000, // 5 seconds
                2 => 10000, // 10 seconds
                3 => 30000, // 30 seconds
                _ => 60000, // 1 minute for 4+
            };
        }
    }
}
