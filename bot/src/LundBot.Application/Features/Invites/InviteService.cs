using LundBot.Application.Common.Caching;
using LundBot.Application.Discord.Guilds;
using LundBot.Application.Discord.Invites;
using LundBot.Application.Discord.Users;
using LundBot.Application.Features.Leaderboards;
using Microsoft.Extensions.DependencyInjection;

namespace LundBot.Application.Features.Invites
{
    public class InviteService : IInviteService
    {
        private readonly IDiscordGuildService _discordGuildService;
        private readonly ICacheService _cacheService;
        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger _logger = Log.ForContext<InviteService>();

        public InviteService(
            IDiscordGuildService discordGuildService,
            ICacheService cacheService,
            IServiceProvider serviceProvider
        )
        {
            _discordGuildService = discordGuildService;
            _cacheService = cacheService;
            _serviceProvider = serviceProvider;
        }

        public async Task<bool> RegisterUserJoinedWithInviteAsync(
            DiscordGuildDto guild,
            DiscordUserDto userJoined,
            DiscordUserDto invitedByUser
        )
        {
            // Discord does not provide a direct way to know who invited a user, so we have to compare the invite uses before and after the user joined.
            var newInvites = await _discordGuildService.GetGuildInvitesAsync(guild.GuildId);

            var oldInvites =
                _cacheService.Get<List<DiscordInviteDto>>(CacheKeys.GuildInvites(guild.GuildId))
                ?? new List<DiscordInviteDto>();

            DiscordInviteDto? usedInvite = newInvites.FirstOrDefault(newInvite =>
                oldInvites.Any(oldInvite =>
                    oldInvite.InviteCode == newInvite.InviteCode && newInvite.Uses > oldInvite.Uses
                )
            );

            // Update cache
            _cacheService.Set(CacheKeys.GuildInvites(guild.GuildId), newInvites.ToList());

            if (usedInvite is null)
            {
                _logger.Information(
                    "User {UserName} ({UserId}) joined guild {GuildName} ({GuildId}) but no invite was used.",
                    userJoined.Username,
                    userJoined.UserId,
                    guild.GuildName,
                    guild.GuildId
                );

                return false;
            }
            if (usedInvite.Inviter is null)
            {
                _logger.Information(
                    "User {UserName} ({UserId}) joined guild {GuildName} ({GuildId}) using invite code {InviteCode} but the inviter is unknown.",
                    userJoined.Username,
                    userJoined.UserId,
                    guild.GuildName,
                    guild.GuildId,
                    usedInvite.InviteCode
                );

                return false;
            }

            DiscordUserDto inviter = usedInvite.Inviter;

            using var scope = _serviceProvider.CreateScope();
            var leaderboardService = scope.ServiceProvider.GetRequiredService<ILeaderboardService>();

            await leaderboardService.RegisterUserJoinedWithInviteAsync(guild, userJoined, inviter);
            return true;
        }
    }
}
