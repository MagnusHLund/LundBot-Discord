using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using LundBot.Helpers;
using LundBot.Interfaces.Services;
using LundBot.Interfaces.Services.Discord;
using Serilog;

namespace LundBot.Services.Discord.Events
{
    public sealed class GuildMemberAddedHandler : IEventHandler<GuildMemberAddedEventArgs>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDiscordGuildService _discordGuildService;
        private readonly ICacheService _cacheService;
        private readonly Serilog.ILogger _logger = Log.ForContext<GuildMemberAddedHandler>();

        public GuildMemberAddedHandler(
            IServiceProvider serviceProvider,
            IDiscordGuildService discordGuildService,
            ICacheService cacheService
        )
        {
            _serviceProvider = serviceProvider;
            _discordGuildService = discordGuildService;
            _cacheService = cacheService;
        }

        public async Task HandleEventAsync(
            DiscordClient sender,
            GuildMemberAddedEventArgs eventArgs
        )
        {
            _logger.Information(
                "Member added: {UserName} ({UserId}) to guild {GuildName} ({GuildId})",
                eventArgs.Member.Username,
                eventArgs.Member.Id,
                eventArgs.Guild.Name,
                eventArgs.Guild.Id
            );

            using var scope = _serviceProvider.CreateScope();
            var welcomeMessageService =
                scope.ServiceProvider.GetRequiredService<IWelcomeMessageService>();

            await welcomeMessageService.SendWelcomeMessageAsync(eventArgs.Guild, eventArgs.Member);
            await RegisterWhoInvitedJoinedUser(eventArgs.Guild, eventArgs.Member);
        }

        private async Task RegisterWhoInvitedJoinedUser(DiscordGuild guild, DiscordMember member)
        {
            // TEMP LOG FOR DEBUGGING ON PROD
            _logger.Information(
                "Registering who invited user {UserName} ({UserId}) in guild {GuildName} ({GuildId})",
                member.Username,
                member.Id,
                guild.Name,
                guild.Id
            );

            // Discord does not provide a direct way to know who invited a user, so we have to compare the invite uses before and after the user joined.
            var newInvites = await _discordGuildService.GetGuildInvitesAsync(guild);

            // TEMP LOG FOR DEBUGGING ON PROD
            _logger.Information(
                "New invites for guild {GuildName} ({GuildId}): {@NewInvites}",
                guild.Name,
                guild.Id,
                newInvites
            );

            var oldInvites =
                _cacheService.Get<List<DiscordInvite>>(
                    CacheKeyHelper.GuildInvites(guild.Id.ToString())
                ) ?? new List<DiscordInvite>();

            // TEMP LOG FOR DEBUGGING ON PROD
            _logger.Information(
                "Old invites for guild {GuildName} ({GuildId}): {@OldInvites}",
                guild.Name,
                guild.Id,
                oldInvites
            );

            DiscordInvite? usedInvite = newInvites.FirstOrDefault(newInvite =>
                oldInvites.Any(oldInvite =>
                    oldInvite.Code == newInvite.Code && newInvite.Uses > oldInvite.Uses
                )
            );

            // TEMP LOG FOR DEBUGGING ON PROD
            _logger.Information(
                "Used invite for guild {GuildName} ({GuildId}): {@UsedInvite}",
                guild.Name,
                guild.Id,
                usedInvite
            );

            // Update cache
            _cacheService.Set(
                CacheKeyHelper.GuildInvites(guild.Id.ToString()),
                newInvites.ToList()
            );

            // TEMP LOG FOR DEBUGGING ON PROD
            _logger.Information(
                "Cache updated for guild {GuildName} ({GuildId}) with new invites: {@NewInvites}",
                guild.Name,
                guild.Id,
                newInvites
            );

            if (usedInvite is null)
            {
                _logger.Information(
                    "User {UserName} ({UserId}) joined guild {GuildName} ({GuildId}) but no invite was used.",
                    member.Username,
                    member.Id,
                    guild.Name,
                    guild.Id
                );

                return;
            }
            if (usedInvite.Inviter is null)
            {
                _logger.Information(
                    "User {UserName} ({UserId}) joined guild {GuildName} ({GuildId}) using invite code {InviteCode} but the inviter is unknown.",
                    member.Username,
                    member.Id,
                    guild.Name,
                    guild.Id,
                    usedInvite.Code
                );

                return;
            }

            _logger.Information(
                "User {UserName} ({UserId}) joined guild {GuildName} ({GuildId}) using invite code {InviteCode} created by {InviterName} ({InviterId})",
                member.Username,
                member.Id,
                guild.Name,
                guild.Id,
                usedInvite.Code,
                usedInvite.Inviter.Username,
                usedInvite.Inviter.Id
            );

            DiscordUser inviter = usedInvite.Inviter;

            // TEMP LOG FOR DEBUGGING ON PROD
            _logger.Information(
                "Preparing to register user {UserName} ({UserId}) with inviter {InviterName} ({InviterId}) in guild {GuildName} ({GuildId})",
                member.Username,
                member.Id,
                inviter.Username,
                inviter.Id,
                guild.Name,
                guild.Id
            );

            using var scope = _serviceProvider.CreateScope();
            var leaderboardService =
                scope.ServiceProvider.GetRequiredService<ILeaderboardService>();

            // TEMP LOG FOR DEBUGGING ON PROD
            _logger.Information(
                "Calling RegisterUserJoinedWithInviteAsync for user {UserName} ({UserId}) with inviter {InviterName} ({InviterId}) in guild {GuildName} ({GuildId})",
                member.Username,
                member.Id,
                inviter.Username,
                inviter.Id,
                guild.Name,
                guild.Id
            );

            await leaderboardService.RegisterUserJoinedWithInviteAsync(guild, member, inviter);

            // TEMP LOG FOR DEBUGGING ON PROD
            _logger.Information(
                "Finished calling RegisterUserJoinedWithInviteAsync for user {UserName} ({UserId}) with inviter {InviterName} ({InviterId}) in guild {GuildName} ({GuildId})",
                member.Username,
                member.Id,
                inviter.Username,
                inviter.Id,
                guild.Name,
                guild.Id
            );
        }
    }
}
