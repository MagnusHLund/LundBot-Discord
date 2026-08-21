using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using LundBot.Config;
using LundBot.Helpers;
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

        private readonly IServiceProvider _serviceProvider;
        private readonly ICommandsService _commandsService;
        private readonly ICacheService _cacheService;
        private readonly IDiscordBotService _discordBotService;
        private readonly IDiscordInteractionService _discordInteractionService;
        private readonly IDiscordGuildService _discordGuildService;
        private readonly IDiscordMemberService _discordMemberService;
        private readonly IModerationActionsService _moderationActionsService;
        private readonly DiscordConfig _discordConfig;
        private readonly ServerConfig _serverConfig;

        public BotService(
            IOptions<DiscordConfig> discordConfig,
            IOptions<ServerConfig> serverConfig,
            IServiceProvider serviceProvider,
            ICommandsService commandsService,
            ICacheService cacheService,
            IDiscordBotService discordBotService,
            IDiscordInteractionService discordInteractionService,
            IDiscordGuildService discordGuildService,
            IDiscordMemberService discordMemberService,
            IModerationActionsService moderationActionsService
        )
        {
            _discordConfig = discordConfig.Value;
            _serverConfig = serverConfig.Value;
            _serviceProvider = serviceProvider;
            _commandsService = commandsService;
            _cacheService = cacheService;
            _moderationActionsService = moderationActionsService;
            _discordBotService = discordBotService;
            _discordInteractionService = discordInteractionService;
            _discordMemberService = discordMemberService;
            _discordGuildService = discordGuildService;
        }

        public async Task InitializeAsync(DiscordClient discordClient)
        {
            _logger.Information(
                "Initializing Bot version {Version} in {Environment} mode...",
                _serverConfig.Version,
                EnvironmentUtils.GetEnvironment()
            );

            DiscordClient = discordClient;

            discordClient.GuildDownloadCompleted += OnGuildDownloadCompleted;
            discordClient.GuildMemberUpdated += OnGuildMemberUpdated;
            discordClient.GuildMemberAdded += OnGuildMemberAdded;
            discordClient.GuildCreated += OnGuildCreated;
            discordClient.Ready += OnClientReady;

            IServiceProvider services = _serviceProvider.CreateScope().ServiceProvider;

            var slash = await _discordBotService.EnableSlashCommands(services);

            slash.SlashCommandExecuted += OnSlashCommandExecuted;
            slash.SlashCommandErrored += OnSlashCommandErrored;

            await _commandsService.RegisterCommandsAsync();

            await _discordBotService.ConnectBotAsync();

            await _commandsService.LogRegisteredCommandsForGuildsAsync();

            _logger.Information("Bot initialization is complete!");
        }

        private async Task SetBotStatusAsync()
        {
            var activity = new DiscordActivity("Stuck in a movie theater", ActivityType.Playing);
            await _discordBotService.UpdateBotStatusAsync(activity);
        }

        private async Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            _logger.Information("Ready fired, running BotService initialization...");
            await SetBotStatusAsync();

            foreach (var guild in sender.Guilds.Values)
            {
                _logger.Information(
                    "Bot is in guild: {GuildName} ({GuildId})",
                    guild.Name,
                    guild.Id
                );

                await _discordMemberService.PreloadMembersAsync(guild);
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

            await AutoKickUserIfRoleAssigned(guild, member);
        }

        private async Task AutoKickUserIfRoleAssigned(DiscordGuild guild, DiscordMember member)
        {
            ulong roleIdToAutoKick = _discordConfig.RoleIdToAutoKick;

            if (roleIdToAutoKick != 0)
            {
                DiscordRole? roleToKick = guild.Roles.Values.FirstOrDefault(r =>
                    r.Id == roleIdToAutoKick
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

        private async Task OnGuildDownloadCompleted(
            DiscordClient sender,
            GuildDownloadCompletedEventArgs e
        )
        {
            foreach (var guild in sender.Guilds.Values)
            {
                var invites = await _discordGuildService.GetGuildInvitesAsync(guild);
                _cacheService.Set(
                    CacheKeyHelper.GuildInvites(guild.Id.ToString()),
                    invites.ToList()
                );
                _logger.Information(
                    "Cached {InviteCount} invites for guild {GuildName} ({GuildId})",
                    invites.Count,
                    guild.Name,
                    guild.Id
                );
            }
        }

        private async Task OnGuildMemberAdded(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            await RegisterWhoInvitedJoinedUser(e.Guild, e.Member);
        }

        private async Task RegisterWhoInvitedJoinedUser(DiscordGuild guild, DiscordUser joinedUser)
        {
            // Discord does not provide a direct way to know who invited a user, so we have to compare the invite uses before and after the user joined.
            var newInvites = await _discordGuildService.GetGuildInvitesAsync(guild);
            var oldInvites =
                _cacheService.Get<List<DiscordInvite>>(
                    CacheKeyHelper.GuildInvites(guild.Id.ToString())
                ) ?? new List<DiscordInvite>();

            var usedInvite = newInvites.FirstOrDefault(newInvite =>
            {
                var oldInvite = oldInvites.FirstOrDefault(i => i.Code == newInvite.Code);
                return oldInvite != null && newInvite.Uses > oldInvite.Uses;
            });

            if (usedInvite != null && usedInvite.Inviter is not null)
            {
                _logger.Information(
                    "User {UserName} ({UserId}) joined guild {GuildName} ({GuildId}) using invite code {InviteCode} created by {InviterName} ({InviterId})",
                    joinedUser.Username,
                    joinedUser.Id,
                    guild.Name,
                    guild.Id,
                    usedInvite.Code,
                    usedInvite.Inviter.Username,
                    usedInvite.Inviter.Id
                );

                DiscordUser inviter = usedInvite.Inviter;

                using var scope = _serviceProvider.CreateScope();
                var leaderboardService =
                    scope.ServiceProvider.GetRequiredService<ILeaderboardService>();

                await leaderboardService.RegisterUserJoinedWithInviteAsync(
                    guild,
                    joinedUser,
                    inviter
                );
            }
            else
            {
                _logger.Information(
                    "User {UserName} ({UserId}) joined guild {GuildName} ({GuildId}) but no invite was used or the inviter is unknown.",
                    joinedUser.Username,
                    joinedUser.Id,
                    guild.Name,
                    guild.Id
                );
            }

            // Update cache
            _cacheService.Set(
                CacheKeyHelper.GuildInvites(guild.Id.ToString()),
                newInvites.ToList()
            );
        }
    }
}
