using System.Text;
using LundBot.Application.Common.Caching;
using LundBot.Application.Common.Exceptions;
using LundBot.Application.Common.Messaging;
using LundBot.Application.Discord.Channels;
using LundBot.Application.Discord.Members;
using LundBot.Application.Discord.Users;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Domain.Leaderboards;

namespace LundBot.Application.Features.Leaderboards.Shared
{
    public abstract class AbstractLeaderboardService : IAbstractLeaderboardService
    {
        private protected int _topScoreLimit = 100;

        private readonly ICacheService _cacheService;
        private readonly ILeaderboardQueue _leaderboardQueue;
        private readonly IDiscordUserService _discordUserService;
        private readonly IDiscordMemberService _discordMemberService;
        private readonly ILeaderboardRepository _leaderboardRepository;
        private readonly ILeaderboardScoreRepository _leaderboardScoreRepository;
        private readonly ILeaderboardMessageRepository _leaderboardMessageRepository;
        private readonly ILeaderboardScoreSourceRepository _leaderboardScoreSourceRepository;
        private readonly IDiscordChannelService _discordChannelService;
        private readonly IMessageService<
            LeaderboardMessage,
            ILeaderboardMessageRepository,
            LeaderboardMessageFactory
        > _messageService;

        private readonly ILogger _logger = Log.ForContext<AbstractLeaderboardService>();

        public AbstractLeaderboardService(
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
        {
            _cacheService = cacheService;
            _messageService = messageService;
            _leaderboardQueue = leaderboardQueue;
            _discordUserService = discordUserService;
            _discordMemberService = discordMemberService;
            _discordChannelService = discordChannelService;
            _leaderboardRepository = leaderboardRepository;
            _leaderboardScoreRepository = leaderboardScoreRepository;
            _leaderboardMessageRepository = leaderboardMessageRepository;
            _leaderboardScoreSourceRepository = leaderboardScoreSourceRepository;
        }

        public async Task<bool> CreateLeaderboardAsync(
            ulong channelId,
            string title,
            string message,
            LeaderboardTypeEnum leaderboardType
        )
        {
            DiscordChannelDto? channel = await _discordChannelService.GetChannelAsync(channelId);
            if (channel is null)
            {
                throw new CommandException($"The channel <#{channelId}> could not be found.", showMessageToUser: true);
            }

            _logger.Information(
                "Creating {LeaderboardType} leaderboard in channel {ChannelId} with title '{Title}' and message '{Message}'",
                leaderboardType,
                channel.ChannelId,
                title,
                message
            );

            var (doesLeaderboardExist, _) = await _leaderboardRepository.DoesLeaderboardExistAsync(
                channel.ChannelId,
                channel.GuildId
            );

            if (doesLeaderboardExist)
            {
                throw new CommandException(
                    $"There can only be one leaderboard per channel. <#{channel.ChannelId}> already has a leaderboard.",
                    showMessageToUser: true
                );
            }

            if (leaderboardType == LeaderboardTypeEnum.Invite)
            {
                var (inviteLeaderboardExists, _) = await _leaderboardRepository.DoesInviteLeaderboardExistOnServerAsync(
                    channel.GuildId
                );

                if (inviteLeaderboardExists)
                {
                    throw new CommandException(
                        $"There can only be one invite leaderboard per server. <#{channel.GuildId}> already has an invite leaderboard.",
                        showMessageToUser: true
                    );
                }
            }

            Leaderboard leaderboard = await _leaderboardRepository.CreateLeaderboardAsync(
                channel.ChannelId,
                channel.GuildId,
                title,
                message,
                leaderboardType
            );

            _messageService.MessageFactory.SetLeaderboardId(leaderboard.Id);

            string leaderboardMessage = await GenerateLeaderboardMessageAsync(
                Enumerable.Empty<LeaderboardScore>(),
                title,
                message,
                channel.GuildId
            );

            bool syncResult = await _messageService.SynchronizeDiscordMessagesAsync(
                leaderboardMessage,
                Enumerable.Empty<LeaderboardMessage>(),
                channel.ChannelId
            );

            if (!syncResult)
            {
                _logger.Warning(
                    "Failed to synchronize leaderboard messages for channel {ChannelId} in guild {GuildId}.",
                    channel.ChannelId,
                    channel.GuildId
                );
            }

            List<Leaderboard> existingLeaderboards = await GetLeaderboardsForGuildAsync(channel.GuildId);
            existingLeaderboards.Add(leaderboard);

            _cacheService.Update<List<Leaderboard>>(
                CacheKeys.LeaderboardsPerGuild(channel.GuildId),
                _ => existingLeaderboards
            );
            return true;
        }

        public async Task<bool> RefreshLeaderboardAsync(ulong channelId, ulong guildId)
        {
            DiscordChannelDto? channel = await _discordChannelService.GetChannelAsync(channelId);

            if (channel is null)
            {
                return false;
            }

            Leaderboard leaderboard = await GetLeaderboardAsync(channelId, channel.GuildId);

            var topUpvoteScores = await _leaderboardScoreRepository.GetTopScoresAsync(leaderboard.Id, _topScoreLimit);

            string leaderboardMessage = await GenerateLeaderboardMessageAsync(
                topUpvoteScores,
                leaderboard.Title,
                leaderboard.Message,
                channel.GuildId
            );

            IEnumerable<LeaderboardMessage> existingMessages =
                await _leaderboardMessageRepository.GetMessagesForLeaderboardAsync(leaderboard.Id);

            return await _messageService.SynchronizeDiscordMessagesAsync(
                leaderboardMessage,
                existingMessages,
                channelId
            );
        }

        public async Task<bool> UpdateLeaderboardMessageAsync(Leaderboard leaderboard, DiscordChannelDto channel)
        {
            var topUpvoteScores = await _leaderboardScoreRepository.GetTopScoresAsync(leaderboard.Id, _topScoreLimit);

            string leaderboardMessage = await GenerateLeaderboardMessageAsync(
                topUpvoteScores,
                leaderboard.Title,
                leaderboard.Message,
                channel.GuildId
            );

            var existingMessages = await _leaderboardMessageRepository.GetMessagesForLeaderboardAsync(leaderboard.Id);

            return await _messageService.SynchronizeDiscordMessagesAsync(
                leaderboardMessage,
                existingMessages,
                channel.ChannelId
            );
        }

        public async Task<bool> RemoveLeaderboardAsync(ulong channelId)
        {
            DiscordChannelDto? channel = await _discordChannelService.GetChannelAsync(channelId);
            if (channel is null)
            {
                throw new CommandException($"The channel <#{channelId}> could not be found.", showMessageToUser: true);
            }

            _logger.Information(
                "Removing leaderboard in channel {ChannelId} for server {GuildId}",
                channelId,
                channel.GuildId
            );

            Leaderboard leaderboard = await GetLeaderboardAsync(channelId, channel.GuildId);

            var existingMessages = await _leaderboardMessageRepository.GetMessagesForLeaderboardAsync(leaderboard.Id);

            bool leaderboardRemoved = await _leaderboardRepository.RemoveLeaderboardAsync(channelId, channel.GuildId);
            bool messagesDeleted = await _messageService.DeleteMessagesForChannelAsync(existingMessages, channelId);

            var existingLeaderboards = await GetLeaderboardsForGuildAsync(channel.GuildId);
            existingLeaderboards.RemoveAll(l => l.Id == leaderboard.Id);

            _cacheService.Update<List<Leaderboard>>(
                CacheKeys.LeaderboardsPerGuild(channel.GuildId),
                _ => existingLeaderboards
            );

            if (!leaderboardRemoved)
            {
                _logger.Warning(
                    "Failed to remove leaderboard in channel {ChannelId} for server {GuildId}",
                    channelId,
                    channel.GuildId
                );
            }

            if (!messagesDeleted)
            {
                _logger.Warning(
                    "Failed to delete messages for leaderboard in channel {ChannelId} for server {GuildId}",
                    channelId,
                    channel.GuildId
                );
            }

            return leaderboardRemoved && messagesDeleted;
        }

        public async ValueTask<List<Leaderboard>> GetLeaderboardsForGuildAsync(ulong guildId)
        {
            List<Leaderboard>? result = _cacheService
                .Get<List<Leaderboard>>(CacheKeys.LeaderboardsPerGuild(guildId))
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

            return await _leaderboardRepository.GetLeaderboardsForGuildAsync(guildId);
        }

        private protected async Task<string> GenerateLeaderboardMessageAsync(
            IEnumerable<LeaderboardScore> topScores,
            string title,
            string message,
            ulong guildId
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
                DiscordMemberDto? member;

                member = await _discordMemberService.GetMemberAsync(guildId, score.DiscordUserId);

                if (member is null)
                {
                    member = await _discordUserService.GetUserAsync(score.DiscordUserId) as DiscordMemberDto;

                    if (member is null)
                    {
                        _logger.Warning(
                            "Failed to retrieve Discord user with ID {DiscordUserId} for leaderboard score with ID {ScoreId}. Skipping this score.",
                            score.DiscordUserId,
                            score.Id
                        );
                        continue;
                    }
                }

                string displayName = member.DisplayName ?? member.GlobalName ?? member.Username;
                sb.AppendLine($"{rank}. **{score.Score}** - **{displayName}** ({member.Username})");
                rank++;
            }

            return sb.ToString();
        }

        private protected async Task<Leaderboard> GetLeaderboardAsync(ulong channelId, ulong guildId)
        {
            (bool doesLeaderboardExist, Leaderboard? leaderboard) =
                await _leaderboardRepository.DoesLeaderboardExistAsync(channelId, guildId);

            if (!doesLeaderboardExist)
            {
                throw new CommandException(
                    $"There is no leaderboard in <#{channelId}> to remove.",
                    showMessageToUser: true
                );
            }

            return leaderboard!;
        }

        private protected async Task AddScoreToLeaderboardAsync(
            ulong userId,
            ulong targetUserId,
            Leaderboard leaderboard
        )
        {
            DiscordChannelDto? leaderboardChannel = await _discordChannelService.GetChannelAsync(
                leaderboard.DiscordChannelId
            );

            if (leaderboardChannel is null)
            {
                throw new CommandException(
                    $"The channel for the leaderboard with ID {leaderboard.Id} could not be found.",
                    showMessageToUser: true
                );
            }

            bool addScoreResult = await _leaderboardScoreSourceRepository.AddScoreAsync(
                userId,
                targetUserId,
                leaderboard.Id
            );
            bool incrementScoreResult = await _leaderboardScoreRepository.IncrementScoreAsync(
                targetUserId,
                leaderboard.Id
            );

            if (!addScoreResult)
            {
                throw new CommandException(
                    $"Failed to add score for user with ID {targetUserId} in leaderboard with ID {leaderboard.Id}.",
                    showMessageToUser: true
                );
            }

            if (!incrementScoreResult)
            {
                throw new CommandException(
                    $"Failed to increment score for user with ID {targetUserId} in leaderboard with ID {leaderboard.Id}.",
                    showMessageToUser: true
                );
            }

            LeaderboardUpdateJob leaderboardUpdateJob = new LeaderboardUpdateJob
            {
                Leaderboard = leaderboard,
                Channel = leaderboardChannel,
            };

            _leaderboardQueue.Enqueue(leaderboardUpdateJob);
        }
    }
}
