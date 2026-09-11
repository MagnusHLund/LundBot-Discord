using LundBot.Application.Common.Exceptions;
using LundBot.Application.Common.Persistence;
using LundBot.Application.Discord.Channels;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Application.Features.Leaderboards.Shared;
using LundBot.Domain.Leaderboards;

namespace LundBot.Application.Features.Leaderboards.Types
{
    public sealed class WarnLeaderboardService : AbstractLeaderboardService, IWarnLeaderboardService
    {
        private readonly IDiscordChannelService _discordChannelService;

        private readonly ILogger _logger = Log.ForContext<WarnLeaderboardService>();

        public WarnLeaderboardService(
            IDiscordChannelService discordChannelService,
            ILeaderboardQueue leaderboardQueue,
            ILeaderboardScoreRepository leaderboardScoreRepository,
            ILeaderboardScoreSourceRepository leaderboardScoreSourceRepository,
            ILeaderboardService leaderboardService,
            IUnitOfWork unitOfWork
        )
            : base(
                discordChannelService,
                leaderboardQueue,
                leaderboardScoreRepository,
                leaderboardScoreSourceRepository,
                leaderboardService,
                unitOfWork
            )
        {
            _discordChannelService = discordChannelService;
        }

        public async Task<bool> RegisterWarningAsync(ulong channelId, ulong senderUserId, ulong targetUserId)
        {
            DiscordChannelDto? channel = await _discordChannelService.GetChannelAsync(channelId);
            if (channel is null)
            {
                throw new CommandException($"The channel <#{channelId}> could not be found.", showMessageToUser: true);
            }

            _logger.Information(
                "Registering a warning for user {UserTargetId} on the leaderboard in channel {ChannelId}",
                targetUserId,
                channelId
            );

            Leaderboard leaderboard = await GetLeaderboardAsync(channelId, channel.GuildId);

            if (leaderboard.LeaderboardType != LeaderboardTypeEnum.Warning)
            {
                throw new CommandException(
                    $"The leaderboard in <#{channelId}> is not a warning leaderboard.",
                    showMessageToUser: true
                );
            }

            await AddScoreToLeaderboardAsync(senderUserId, targetUserId, leaderboard);
            return true;
        }
    }
}
