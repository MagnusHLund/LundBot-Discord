using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using LundBot.Interfaces.Services;
using LundBot.Services;
using Serilog;

namespace LundBot.BackgroundServices
{
    public sealed class DiscordBotBackgroundService : BackgroundService
    {
        private readonly DiscordClient _discordClient;
        private readonly IBotService _botService;
        private readonly ICommandsService _commandsService;
        private readonly Serilog.ILogger _logger = Log.ForContext<DiscordBotBackgroundService>();

        public DiscordBotBackgroundService(
            DiscordClient discordClient,
            IBotService botService,
            ICommandsService commandsService
        )
        {
            _discordClient = discordClient;
            _botService = botService;
            _commandsService = commandsService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            BotService.DiscordClient = _discordClient;

            _discordClient.GuildCreated += OnGuildCreated;
            _discordClient.Ready += OnClientReady;

            _logger.Information("Creating SlashCommandsExtension and registering commands...");

            var slash = _discordClient.UseSlashCommands();

            slash.SlashCommandExecuted += OnSlashCommandExecuted;
            slash.SlashCommandErrored += OnSlashCommandErrored;

            await _commandsService.RegisterCommandsAsync();

            _logger.Information("Connecting to Discord...");
            await _discordClient.ConnectAsync();

            await _commandsService.LogRegisteredCommandsForGuildsAsync();

            _logger.Information("Bot initialization is complete!");
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            _logger.Information("Ready fired, running BotService initialization...");
            try
            {
                await _botService.InitializeAsync(_discordClient);
                _logger.Information("BotService initialization completed.");
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "BotService.InitializeAsync threw.");
            }
        }

        private async Task OnSlashCommandExecuted(
            SlashCommandsExtension sender,
            SlashCommandExecutedEventArgs e
        )
        {
            _logger.Information(
                "Slash executed: {Cmd} by {User} in Guild={Guild}",
                e.Context.CommandName,
                e.Context.User?.Username,
                e.Context.Guild?.Id ?? 0
            );
        }

        private async Task OnSlashCommandErrored(
            SlashCommandsExtension sender,
            SlashCommandErrorEventArgs e
        )
        {
            _logger.Error(
                e.Exception,
                "Slash errored: {Cmd} by {User} in Guild={Guild}",
                e.Context?.CommandName ?? "<unknown>",
                e.Context?.User?.Username ?? "<unknown>",
                e.Context?.Guild?.Id ?? 0
            );

            try
            {
                if (e.Context != null)
                {
                    await e.Context.CreateResponseAsync(
                        InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder()
                            .WithContent("Internal error")
                            .AsEphemeral(true)
                    );
                }
            }
            catch { }
        }

        private async Task OnGuildCreated(DiscordClient sender, GuildCreateEventArgs e)
        {
            _logger.Information("Guild created: {GuildName} ({GuildId})", e.Guild.Name, e.Guild.Id);

            try
            {
                await _commandsService.RefreshCommands();
                _logger.Information("Registered commands for guild {GuildId}", e.Guild.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error registering commands for guild {GuildId}", e.Guild.Id);
            }
        }
    }
}
