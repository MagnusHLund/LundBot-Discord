using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using LundBot.Config;
using LundBot.Interfaces.Services;
using Microsoft.Extensions.Options;
using Serilog;

namespace LundBot.Services
{
    public sealed class BotService : IBotService
    {
        public static DiscordClient DiscordClient { get; set; } = null!;
        private readonly Serilog.ILogger _logger = Log.ForContext<BotService>();

        private readonly IServiceProvider _serviceProvider;
        private readonly ICommandsService _commandsService;
        private readonly IModerationActionsService _moderationActionsService;
        private readonly DiscordConfig _discordConfig;

        public BotService(
            IOptions<DiscordConfig> options,
            IServiceProvider serviceProvider,
            ICommandsService commandsService,
            IModerationActionsService moderationActionsService
        )
        {
            _discordConfig = options.Value;
            _serviceProvider = serviceProvider;
            _commandsService = commandsService;
            _moderationActionsService = moderationActionsService;
        }

        public async Task InitializeAsync(DiscordClient discordClient)
        {
            DiscordClient = discordClient;

            discordClient.GuildMemberUpdated += OnGuildMemberUpdated;
            discordClient.GuildCreated += OnGuildCreated;
            discordClient.Ready += OnClientReady;

            _logger.Information("Creating SlashCommandsExtension and registering commands...");

            var slash = discordClient.UseSlashCommands(
                new SlashCommandsConfiguration
                {
                    Services = _serviceProvider.CreateScope().ServiceProvider,
                }
            );

            slash.SlashCommandExecuted += OnSlashCommandExecuted;
            slash.SlashCommandErrored += OnSlashCommandErrored;

            await _commandsService.RegisterCommandsAsync();

            _logger.Information("Connecting to Discord...");
            await discordClient.ConnectAsync();

            await _commandsService.LogRegisteredCommandsForGuildsAsync();

            _logger.Information("Bot initialization is complete!");
        }

        private async Task SetBotStatusAsync()
        {
            var activity = new DiscordActivity("Stuck in a movie theater", ActivityType.Playing);
            await DiscordClient.UpdateStatusAsync(activity, UserStatus.Online);
        }

        private async Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            _logger.Information("Ready fired, running BotService initialization...");
            try
            {
                await SetBotStatusAsync();
                ;
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

        private async Task OnGuildMemberUpdated(DiscordClient sender, GuildMemberUpdateEventArgs e)
        {
            DiscordGuild guild = e.Guild;
            DiscordMember member = e.Member;

            string roleIdToAutoKick = _discordConfig.RoleIdToAutoKick;

            if (!string.IsNullOrEmpty(roleIdToAutoKick))
            {
                DiscordRole? roleToKick = guild.Roles.Values.FirstOrDefault(r =>
                    r.Id.ToString() == roleIdToAutoKick
                );

                string kickReason =
                    $"You have been automatically kicked due to picking the \"{roleToKick?.Name ?? "Unknown"}\" role. This community is PC only. Feel free to join back, if you own IW on PC or plan to purchase it on PC";

                await _moderationActionsService.KickUserDueToRoleAssignmentAsync(
                    guild,
                    member,
                    roleToKick,
                    kickReason
                );
            }
        }
    }
}
