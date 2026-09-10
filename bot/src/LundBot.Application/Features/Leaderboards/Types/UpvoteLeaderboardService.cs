using LundBot.Application.Common.Caching;
using LundBot.Application.Common.Exceptions;
using LundBot.Application.Common.Messaging;
using LundBot.Application.Discord.Channels;
using LundBot.Application.Discord.Members;
using LundBot.Application.Discord.Users;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Application.Features.Leaderboards.Shared;
using LundBot.Domain.Leaderboards;

namespace LundBot.Application.Features.Leaderboards.Types
{
    public sealed class UpvoteLeaderboardService : AbstractLeaderboardService, IUpvoteLeaderboardService
    {
        private readonly IDiscordChannelService _discordChannelService;
        private readonly ILeaderboardScoreSourceRepository _leaderboardScoreSourceRepository;

        private readonly ILogger _logger = Log.ForContext<UpvoteLeaderboardService>();

        public UpvoteLeaderboardService(
            IDiscordUserService discordUserService,
            IDiscordMemberService discordMemberService,
            ILeaderboardRepository leaderboardRepository,
            IMessageService<
                LeaderboardMessage,
                ILeaderboardMessageRepository,
                LeaderboardMessageFactory
            > messageService,
            ICacheService cacheService,
            IDiscordChannelService discordChannelService,
            ILeaderboardQueue leaderboardQueue,
            ILeaderboardScoreRepository leaderboardScoreRepository,
            ILeaderboardMessageRepository leaderboardMessageRepository,
            ILeaderboardScoreSourceRepository leaderboardScoreSourceRepository
        )
            : base(
                discordUserService,
                discordMemberService,
                leaderboardRepository,
                messageService,
                cacheService,
                discordChannelService,
                leaderboardQueue,
                leaderboardScoreRepository,
                leaderboardMessageRepository,
                leaderboardScoreSourceRepository
            )
        {
            _discordChannelService = discordChannelService;
            _leaderboardScoreSourceRepository = leaderboardScoreSourceRepository;
        }

        public async Task<bool> UpvoteUserAsync(ulong channelId, DiscordUserDto userUpvoting, DiscordUserDto targetUser)
        {
            DiscordChannelDto? channel = await _discordChannelService.GetChannelAsync(channelId);
            if (channel is null)
            {
                throw new CommandException($"The channel <#{channelId}> could not be found.", showMessageToUser: true);
            }

            _logger.Information(
                "User {UserUpvotingId} is upvoting user {UserTargetId} on the leaderboard in channel {ChannelId}",
                userUpvoting.UserId,
                targetUser.UserId,
                channelId
            );

            Leaderboard leaderboard = await GetLeaderboardAsync(channelId, channel.GuildId);

            if (leaderboard.LeaderboardType != LeaderboardTypeEnum.Upvote)
            {
                throw new CommandException(
                    $"The leaderboard in <#{channelId}> is not an upvote leaderboard.",
                    showMessageToUser: true
                );
            }

            bool hasAlreadyUpvoted = await _leaderboardScoreSourceRepository.HasUserGivenScoreToTargetAsync(
                userUpvoting.UserId,
                targetUser.UserId,
                leaderboard.Id
            );

            if (hasAlreadyUpvoted)
            {
                throw new CommandException(
                    $"You have already upvoted {targetUser.Username} on the leaderboard in <#{channelId}>.",
                    showMessageToUser: true
                );
            }

            await AddScoreToLeaderboardAsync(userUpvoting.UserId, targetUser.UserId, leaderboard);
            return true;
        }
    }
}
