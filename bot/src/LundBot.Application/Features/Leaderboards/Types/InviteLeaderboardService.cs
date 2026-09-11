using LundBot.Application.Discord.Channels;
using LundBot.Application.Discord.Guilds;
using LundBot.Application.Discord.Roles;
using LundBot.Application.Discord.Users;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Application.Features.Leaderboards.Shared;
using LundBot.Domain.Leaderboards;
using Microsoft.Extensions.Hosting;

namespace LundBot.Application.Features.Leaderboards.Types
{
    public sealed class InviteLeaderboardService : AbstractLeaderboardService, IInviteLeaderboardService
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IDiscordRoleService _discordRoleService;
        private readonly ILeaderboardRepository _leaderboardRepository;
        private readonly ILeaderboardScoreSourceRepository _leaderboardScoreSourceRepository;

        private readonly ILogger _logger = Log.ForContext<InviteLeaderboardService>();

        public InviteLeaderboardService(
            IHostEnvironment hostEnvironment,
            IDiscordRoleService discordRoleService,
            ILeaderboardRepository leaderboardRepository,
            ILeaderboardScoreSourceRepository leaderboardScoreSourceRepository,
            ILeaderboardScoreRepository leaderboardScoreRepository,
            IDiscordChannelService discordChannelService,
            ILeaderboardQueue leaderboardQueue,
            ILeaderboardService leaderboardService
        )
            : base(
                discordChannelService,
                leaderboardQueue,
                leaderboardScoreRepository,
                leaderboardScoreSourceRepository,
                leaderboardService
            )
        {
            _hostEnvironment = hostEnvironment;
            _discordRoleService = discordRoleService;
            _leaderboardRepository = leaderboardRepository;
            _leaderboardScoreSourceRepository = leaderboardScoreSourceRepository;
        }

        public async Task<bool> RegisterSuccessfullyInvitedUserAsync(
            DiscordGuildDto guild,
            DiscordUserDto userJoined,
            DiscordUserDto userInvitedBy
        )
        {
            if (
                _hostEnvironment.IsProduction()
                    && await _discordRoleService.IsMemberOwnerAsync(userInvitedBy.UserId, guild.GuildId)
                || await _discordRoleService.IsMemberABotAsync(userInvitedBy.UserId, guild.GuildId)
            )
            {
                _logger.Information(
                    "User {UserInvitedById} is either the owner or a bot in guild {GuildId}, skipping registration of user {UserJoinedId}",
                    userInvitedBy.UserId,
                    guild.GuildId,
                    userJoined.UserId
                );

                return false;
            }

            (bool leaderboardExists, Leaderboard? leaderboard) =
                await _leaderboardRepository.DoesInviteLeaderboardExistOnServerAsync(guild.GuildId);

            if (!leaderboardExists)
            {
                _logger.Information(
                    "No invite leaderboard exists in guild {GuildId}, skipping registration of user {UserJoinedId}",
                    guild.GuildId,
                    userJoined.UserId
                );
                return false;
            }

            bool hasAlreadyBeenInvited = await _leaderboardScoreSourceRepository.HasUserGivenScoreToTargetAsync(
                userJoined.UserId,
                userInvitedBy.UserId,
                leaderboard!.Id
            );

            if (hasAlreadyBeenInvited)
            {
                _logger.Information(
                    "User {UserJoinedId} has already been invited by {UserInvitedById} on the invite leaderboard in guild {GuildId}, skipping registration",
                    userJoined.UserId,
                    userInvitedBy.UserId,
                    guild.GuildId
                );
                return false;
            }

            await AddScoreToLeaderboardAsync(userJoined.UserId, userInvitedBy.UserId, leaderboard);
            return true;
        }
    }
}
