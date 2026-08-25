using System.Reflection;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.EventArgs;
using LundBot.Config;
using LundBot.Interfaces.Services;
using LundBot.Interfaces.Services.Discord;
using LundBot.Utils;
using Microsoft.Extensions.Options;
using Serilog;

namespace LundBot.Services
{
    public sealed class BotService : IBotService
    {
        public static DiscordClient DiscordClient { get; set; } = null!;
        private readonly Serilog.ILogger _logger = Log.ForContext<BotService>();

        private readonly ICommandsService _commandsService;
        private readonly IDiscordBotService _discordBotService;
        private readonly IDiscordInteractionService _discordInteractionService;
        private readonly ServerConfig _serverConfig;

        public BotService(
            IOptions<ServerConfig> serverConfig,
            ICommandsService commandsService,
            IDiscordBotService discordBotService,
            IDiscordInteractionService discordInteractionService
        )
        {
            _serverConfig = serverConfig.Value;
            _commandsService = commandsService;
            _discordBotService = discordBotService;
            _discordInteractionService = discordInteractionService;
        }

        public async Task InitializeAsync(DiscordClient discordClient)
        {
            string dSharpPlusVersion =
                typeof(DiscordClient)
                    .Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion
                ?? "Unknown";

            _logger.Information(
                "Initializing Bot version {Version} in {Environment} mode... (DSharpPlus version: {DSharpPlusVersion})",
                _serverConfig.Version,
                EnvironmentUtils.GetEnvironment(),
                dSharpPlusVersion
            );

            DiscordClient = discordClient;

            await _commandsService.RegisterCommandsAsync();

            await _discordBotService.ConnectBotAsync();

            await _commandsService.LogRegisteredCommandsForGuildsAsync();

            _logger.Information("Bot initialization is complete!");
        }

        public async Task OnSlashCommandExecuted(
            CommandsExtension sender,
            CommandExecutedEventArgs e
        )
        {
            _logger.Information(
                "Slash executed: {Cmd} by {User} in Guild={Guild}",
                e.Context.Command.Name,
                e.Context.User?.Username,
                e.Context.Guild?.Id ?? 0
            );
        }

        public async Task OnSlashCommandErrored(CommandsExtension sender, CommandErroredEventArgs e)
        {
            _logger.Error(
                e.Exception,
                "Slash errored: {Cmd} by {User} in Guild={Guild}",
                e.Context?.Command.Name ?? "<unknown>",
                e.Context?.User?.Username ?? "<unknown>",
                e.Context?.Guild?.Id ?? 0
            );

            try
            {
                if (e.Context != null)
                {
                    await _discordInteractionService.SendResponseAsync(
                        e.Context,
                        "Internal server error. Please try again later.",
                        showOnlyToUser: true
                    );
                }
                else
                {
                    _logger.Warning("Cannot send error response because the context is null.");
                }
            }
            catch { }
        }
    }
}
