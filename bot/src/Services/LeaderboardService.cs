using System.Text;
using DSharpPlus.Entities;
using LundBot.Entities;
using LundBot.Enums;
using LundBot.Exceptions;
using LundBot.Factories.MessageEntityFactories;
using LundBot.Interfaces.Repositories;
using LundBot.Interfaces.Services;
using LundBot.Repositories;

namespace LundBot.Services
{
    public sealed class LeaderboardService : ILeaderboardService
    {
        private readonly ILeaderboardMessagesRepository _leaderboardsMessageRepository;
        private readonly IUpvotingLeaderboardRepository _upvotingLeaderboardRepository;
        private readonly ILeaderboardScoresRepository _leaderboardScoreRepository;
        private readonly ILeaderboardsRepository _leaderboardsRepository;
        private readonly IMessageService<
            LeaderboardMessagesEntity,
            LeaderboardMessagesRepository,
            LeaderboardMessageFactory
        > _messageService;
        private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<LeaderboardService>();

        public LeaderboardService(
            ILeaderboardsRepository leaderboardsRepository,
            ILeaderboardMessagesRepository leaderboardsMessageRepository,
            IUpvotingLeaderboardRepository upvotingLeaderboardRepository,
            ILeaderboardScoresRepository leaderboardScoreRepository,
            IMessageService<
                LeaderboardMessagesEntity,
                LeaderboardMessagesRepository,
                LeaderboardMessageFactory
            > messageService
        )
        {
            _leaderboardsRepository = leaderboardsRepository;
            _leaderboardsMessageRepository = leaderboardsMessageRepository;
            _upvotingLeaderboardRepository = upvotingLeaderboardRepository;
            _leaderboardScoreRepository = leaderboardScoreRepository;
            _messageService = messageService;
        }

        public async Task CreateUpvoteLeaderboardAsync(
            DiscordChannel channel,
            string title,
            string message
        )
        {
            await CreateLeaderboardAsync(channel, title, message, LeaderboardType.Upvote);
        }

        public async Task CreateInviteLeaderboardAsync(
            DiscordChannel channel,
            string title,
            string message
        )
        {
            await CreateLeaderboardAsync(channel, title, message, LeaderboardType.Invite);
        }

        public async Task RemoveLeaderboardAsync(DiscordChannel channel)
        {
            _logger.Information(
                "Removing leaderboard in channel {ChannelId} for server {GuildId}",
                channel.Id,
                channel.Guild.Id
            );

            LeaderboardsEntity leaderboard = await GetLeaderboardAsync(channel);

            var existingMessages =
                await _leaderboardsMessageRepository.GetMessagesForLeaderboardAsync(leaderboard.Id);

            await _leaderboardsRepository.RemoveLeaderboardAsync(
                channel.Id.ToString(),
                channel.Guild.Id.ToString()
            );

            await _messageService.DeleteMessagesForChannelAsync(existingMessages, channel);
        }

        public async Task UpvoteUserOnLeaderboard(
            DiscordChannel channel,
            DiscordUser userUpvoting,
            DiscordUser userTarget
        )
        {
            _logger.Information(
                "User {UserUpvotingId} is upvoting user {UserTargetId} on the leaderboard in channel {ChannelId}",
                userUpvoting.Id,
                userTarget.Id,
                channel.Id
            );

            LeaderboardsEntity leaderboard = await GetLeaderboardAsync(channel);

            if (leaderboard.LeaderboardType != LeaderboardType.Upvote)
            {
                throw new CommandException(
                    $"The leaderboard in <#{channel.Id}> is not an upvote leaderboard.",
                    showMessageToUser: true
                );
            }

            bool hasAlreadyUpvoted = await _upvotingLeaderboardRepository.HasUserUpvotedTargetAsync(
                userUpvoting.Id.ToString(),
                userTarget.Id.ToString(),
                leaderboard.Id
            );

            if (hasAlreadyUpvoted)
            {
                throw new CommandException(
                    $"You have already upvoted {userTarget.Username} on the leaderboard in <#{channel.Id}>.",
                    showMessageToUser: true
                );
            }

            await _upvotingLeaderboardRepository.AddUpvoteAsync(
                userUpvoting.Id.ToString(),
                userTarget.Id.ToString(),
                leaderboard.Id
            );

            await _leaderboardScoreRepository.IncrementScoreAsync(
                userTarget.Id.ToString(),
                leaderboard.Id
            );

            const int topUpvoteScoresLimit = 100;
            var topUpvoteScores = await _leaderboardScoreRepository.GetTopScoresAsync(
                leaderboard.Id,
                topUpvoteScoresLimit
            );

            string leaderboardMessage = GenerateLeaderboardMessage(
                topUpvoteScores,
                leaderboard.Title,
                leaderboard.Message
            );

            var existingMessages =
                await _leaderboardsMessageRepository.GetMessagesForLeaderboardAsync(leaderboard.Id);

            await _messageService.SynchronizeDiscordMessagesAsync(
                leaderboardMessage,
                existingMessages,
                channel.Id
            );
        }

        private async Task CreateLeaderboardAsync(
            DiscordChannel channel,
            string title,
            string message,
            LeaderboardType leaderboardType
        )
        {
            _logger.Information(
                "Creating {LeaderboardType} leaderboard in channel {ChannelId} with title '{Title}' and message '{Message}'",
                leaderboardType,
                channel.Id,
                title,
                message
            );

            var (doesLeaderboardExist, _) = await _leaderboardsRepository.DoesLeaderboardExistAsync(
                channel.Id.ToString(),
                channel.Guild.Id.ToString()
            );

            if (doesLeaderboardExist)
            {
                throw new CommandException(
                    $"There can only be one leaderboard per channel. <#{channel.Id}> already has a leaderboard.",
                    showMessageToUser: true
                );
            }

            LeaderboardsEntity leaderboard = await _leaderboardsRepository.CreateLeaderboardAsync(
                channel.Id.ToString(),
                channel.Guild.Id.ToString(),
                title,
                message,
                leaderboardType
            );

            _messageService.MessageFactory.SetLeaderboardId(leaderboard.Id);

            string leaderboardMessage = GenerateLeaderboardMessage(
                Enumerable.Empty<LeaderboardScoresEntity>(),
                title,
                message
            );

            await _messageService.SynchronizeDiscordMessagesAsync(
                leaderboardMessage,
                Enumerable.Empty<LeaderboardMessagesEntity>(),
                channel.Id
            );
        }

        private string GenerateLeaderboardMessage(
            IEnumerable<LeaderboardScoresEntity> topScores,
            string title,
            string message
        )
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"**{message}**");
            sb.AppendLine($"# {title}");
            sb.AppendLine();

            if (!topScores.Any())
            {
                sb.AppendLine("Empty leaderboard. Be the first to reach the top!");
                return sb.ToString();
            }

            int rank = 1;
            foreach (var score in topScores)
            {
                sb.AppendLine($"{rank}. <@{score.DiscordUserId}> - {score.Score}");
                rank++;
            }

            return sb.ToString();
        }

        private async Task<LeaderboardsEntity> GetLeaderboardAsync(DiscordChannel channel)
        {
            (bool doesLeaderboardExist, LeaderboardsEntity? leaderboard) =
                await _leaderboardsRepository.DoesLeaderboardExistAsync(
                    channel.Id.ToString(),
                    channel.Guild.Id.ToString()
                );

            if (!doesLeaderboardExist)
            {
                throw new CommandException(
                    $"There is no leaderboard in <#{channel.Id}> to remove.",
                    showMessageToUser: true
                );
            }

            return leaderboard;
        }
    }
}
