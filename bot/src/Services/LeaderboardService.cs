using System.Text;
using DSharpPlus.Entities;
using LundBot.Entities;
using LundBot.Enums;
using LundBot.Exceptions;
using LundBot.Factories.MessageEntityFactories;
using LundBot.Helpers;
using LundBot.Interfaces.Repositories;
using LundBot.Interfaces.Services;
using LundBot.Repositories;
using LundBot.Utils;

namespace LundBot.Services
{
    public sealed class LeaderboardService : ILeaderboardService
    {
        private const int TOP_UPVOTE_SCORES_LIMIT = 100;

        private readonly IUserService _userService;
        private readonly ILeaderboardMessagesRepository _leaderboardsMessageRepository;
        private readonly ILeaderboardScoreSourceRepository _leaderboardScoreSourceRepository;
        private readonly ILeaderboardScoresRepository _leaderboardScoreRepository;
        private readonly ICacheService _cacheService;
        private readonly ILeaderboardsRepository _leaderboardsRepository;
        private readonly IMessageService<
            LeaderboardMessagesEntity,
            LeaderboardMessagesRepository,
            LeaderboardMessageFactory
        > _messageService;
        private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<LeaderboardService>();

        public LeaderboardService(
            IUserService userService,
            ILeaderboardsRepository leaderboardsRepository,
            ILeaderboardMessagesRepository leaderboardsMessageRepository,
            ILeaderboardScoreSourceRepository leaderboardScoreSourceRepository,
            ILeaderboardScoresRepository leaderboardScoreRepository,
            ICacheService cacheService,
            IMessageService<
                LeaderboardMessagesEntity,
                LeaderboardMessagesRepository,
                LeaderboardMessageFactory
            > messageService
        )
        {
            _userService = userService;
            _leaderboardsRepository = leaderboardsRepository;
            _leaderboardsMessageRepository = leaderboardsMessageRepository;
            _leaderboardScoreSourceRepository = leaderboardScoreSourceRepository;
            _leaderboardScoreRepository = leaderboardScoreRepository;
            _messageService = messageService;
            _cacheService = cacheService;
        }

        public async Task CreateLeaderboardAsync(
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

            if (leaderboardType == LeaderboardType.Invite)
            {
                var (inviteLeaderboardExists, _) =
                    await _leaderboardsRepository.DoesInviteLeaderboardExistOnServerAsync(
                        channel.Guild.Id.ToString()
                    );

                if (inviteLeaderboardExists)
                {
                    throw new CommandException(
                        $"There can only be one invite leaderboard per server. <#{channel.Guild.Id}> already has an invite leaderboard.",
                        showMessageToUser: true
                    );
                }
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

            var existingLeaderboards = await GetLeaderboardsForGuildAsync(
                channel.Guild.Id.ToString()
            );
            existingLeaderboards.Add(leaderboard);

            _cacheService.Update<List<LeaderboardsEntity>>(
                CacheKeyHelper.LeaderboardsPerGuild(channel.Guild.Id.ToString()),
                _ => existingLeaderboards
            );
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

            var existingLeaderboards = await GetLeaderboardsForGuildAsync(
                channel.Guild.Id.ToString()
            );
            existingLeaderboards.RemoveAll(l => l.Id == leaderboard.Id);

            _cacheService.Update<List<LeaderboardsEntity>>(
                CacheKeyHelper.LeaderboardsPerGuild(channel.Guild.Id.ToString()),
                _ => existingLeaderboards
            );
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

            bool hasAlreadyUpvoted =
                await _leaderboardScoreSourceRepository.HasUserGivenScoreToTargetAsync(
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

            await AddScoreToLeaderboardAsync(
                userUpvoting.Id.ToString(),
                userTarget.Id.ToString(),
                leaderboard,
                channel
            );
        }

        public async Task RegisterUserJoinedWithInvite(
            DiscordGuild guild,
            DiscordUser userJoined,
            DiscordUser userInvitedBy
        )
        {
            if (
                EnvironmentUtils.IsProduction()
                    && await _userService.IsUserOwnerAsync(userInvitedBy.Id, guild.Id)
                || await _userService.IsUserABot(userInvitedBy.Id, guild.Id)
            )
            {
                return;
            }

            (bool leaderboardExists, LeaderboardsEntity? leaderboard) =
                await _leaderboardsRepository.DoesInviteLeaderboardExistOnServerAsync(
                    guild.Id.ToString()
                );

            if (!leaderboardExists)
            {
                _logger.Information(
                    "No invite leaderboard exists in guild {GuildId}, skipping registration of user {UserJoinedId}",
                    guild.Id,
                    userJoined.Id
                );
                return;
            }

            bool hasAlreadyBeenInvited =
                await _leaderboardScoreSourceRepository.HasUserGivenScoreToTargetAsync(
                    userInvitedBy.Id.ToString(),
                    userJoined.Id.ToString(),
                    leaderboard!.Id
                );

            if (hasAlreadyBeenInvited)
            {
                _logger.Information(
                    "User {UserJoinedId} has already been invited by {UserInvitedById} on the invite leaderboard in guild {GuildId}, skipping registration",
                    userJoined.Id,
                    userInvitedBy.Id,
                    guild.Id
                );
                return;
            }

            await AddScoreToLeaderboardAsync(
                userJoined.Id.ToString(),
                userInvitedBy.Id.ToString(),
                leaderboard
            );
        }

        public async Task RefreshLeaderboardAsync(ulong channelId, ulong guildId)
        {
            DiscordChannel channel;

            try
            {
                channel = await BotService.DiscordClient.GetChannelAsync(channelId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while fetching channel {ChannelId}", channelId);
                throw;
            }

            LeaderboardsEntity leaderboard = await GetLeaderboardAsync(channel);

            var topUpvoteScores = await _leaderboardScoreRepository.GetTopScoresAsync(
                leaderboard.Id,
                TOP_UPVOTE_SCORES_LIMIT
            );

            string leaderboardMessage = GenerateLeaderboardMessage(
                topUpvoteScores,
                leaderboard.Title,
                leaderboard.Message
            );

            List<LeaderboardMessagesEntity> existingMessages =
                await _leaderboardsMessageRepository.GetMessagesForLeaderboardAsync(leaderboard.Id);

            await _messageService.SynchronizeDiscordMessagesAsync(
                leaderboardMessage,
                existingMessages,
                channelId
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

        public async ValueTask<List<LeaderboardsEntity>> GetLeaderboardsForGuildAsync(
            string guildId
        )
        {
            var result = _cacheService
                .Get<List<LeaderboardsEntity>>(CacheKeyHelper.LeaderboardsPerGuild(guildId))
                ?.ToList();

            if (result is not null)
            {
                _logger.Information(
                    "Retrieved {Count} leaderboards for guild {GuildId} from cache",
                    result.Count,
                    guildId
                );

                return result;
            }

            return await _leaderboardsRepository.GetLeaderboardsForGuildAsync(guildId);
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

            return leaderboard!;
        }

        private async Task AddScoreToLeaderboardAsync(
            string userId,
            string targetUserId,
            LeaderboardsEntity leaderboard,
            DiscordChannel? channel = null
        )
        {
            if (channel is null)
            {
                channel = await BotService.DiscordClient.GetChannelAsync(
                    ulong.Parse(leaderboard.DiscordChannelId)
                );
            }

            await _leaderboardScoreSourceRepository.AddScoreAsync(
                userId,
                targetUserId,
                leaderboard.Id
            );

            await _leaderboardScoreRepository.IncrementScoreAsync(targetUserId, leaderboard.Id);

            var topUpvoteScores = await _leaderboardScoreRepository.GetTopScoresAsync(
                leaderboard.Id,
                TOP_UPVOTE_SCORES_LIMIT
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
    }
}
