using System.Text;
using LundBot.Application.Common.Caching;
using LundBot.Application.Common.Exceptions;
using LundBot.Application.Common.Messaging;
using LundBot.Application.Discord.Channels;
using LundBot.Application.Discord.Guilds;
using LundBot.Application.Discord.Members;
using LundBot.Application.Discord.Roles;
using LundBot.Application.Discord.Users;
using LundBot.Domain.Leaderboards;
using Microsoft.Extensions.Hosting;

namespace LundBot.Application.Features.Leaderboards
{
    public sealed class LeaderboardService : ILeaderboardService
    {
        private const int TOP_UPVOTE_SCORES_LIMIT = 100;

        private readonly IHostEnvironment _hostEnvironment;
        private readonly IDiscordRoleService _discordRoleService;
        private readonly ILeaderboardRepository _leaderboardRepository;
        private readonly ILeaderboardScoreRepository _leaderboardScoreRepository;
        private readonly ILeaderboardScoreSourceRepository _leaderboardScoreSourceRepository;
        private readonly IDiscordMemberService _discordMemberService;
        private readonly ILeaderboardMessageRepository _leaderboardMessageRepository;
        private readonly IDiscordUserService _discordUserService;
        private readonly IDiscordChannelService _discordChannelService;
        private readonly ILeaderboardQueue _leaderboardQueue;
        private readonly ICacheService _cacheService;
        private readonly IMessageService<
            ILeaderboardMessageRepository,
            LeaderboardMessageFactory,
            LeaderboardMessage
        > _messageService;

        private readonly ILogger _logger = Log.ForContext<LeaderboardService>();

        public LeaderboardService(
            ILeaderboardRepository leaderboardRepository,
            ILeaderboardScoreRepository leaderboardScoreRepository,
            ILeaderboardScoreSourceRepository leaderboardScoreSourceRepository,
            ICacheService cacheService,
            IDiscordChannelService discordChannelService,
            ILeaderboardQueue leaderboardQueue,
            IDiscordMemberService discordMemberService,
            IDiscordUserService discordUserService,
            IMessageService<LeaderboardMessageRepository, LeaderboardMessageFactory, LeaderboardMessage> messageService
        )
        {
            _leaderboardRepository = leaderboardRepository;
            _leaderboardScoreRepository = leaderboardScoreRepository;
            _leaderboardScoreSourceRepository = leaderboardScoreSourceRepository;
            _discordChannelService = discordChannelService;
            _leaderboardQueue = leaderboardQueue;
            _cacheService = cacheService;
            _discordChannelService = discordChannelService;
            _leaderboardQueue = leaderboardQueue;
            _messageService = messageService;
            _discordMemberService = discordMemberService;
            _discordUserService = discordUserService;
        }

        public async Task<bool> CreateLeaderboardAsync(
            DiscordChannelDto channel,
            string title,
            string message,
            LeaderboardTypeEnum leaderboardType
        )
        {
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

            await _messageService.SynchronizeDiscordMessagesAsync(
                leaderboardMessage,
                Enumerable.Empty<LeaderboardMessage>(),
                channel.ChannelId
            );

            var existingLeaderboards = await GetLeaderboardsForGuildAsync(channel.GuildId);
            existingLeaderboards.Add(leaderboard);

            _cacheService.Update<List<Leaderboard>>(
                CacheKeys.LeaderboardsPerGuild(channel.GuildId),
                _ => existingLeaderboards
            );
            return true;
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

        public async Task<bool> RefreshLeaderboardAsync(ulong channelId, ulong guildId)
        {
            DiscordChannelDto? channel = await _discordChannelService.GetChannelAsync(channelId);

            if (channel is null)
            {
                return false;
            }

            Leaderboard leaderboard = await GetLeaderboardAsync(channel);

            var topUpvoteScores = await _leaderboardScoreRepository.GetTopScoresAsync(
                leaderboard.Id,
                TOP_UPVOTE_SCORES_LIMIT
            );

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

        // TODO: This does not have a sender
        public async Task<bool> RegisterWarningOnLeaderboardAsync(ulong channelId, ulong guildId, DiscordUserDto targetUser)
        {
            _logger.Information(
                "Registering a warning for user {UserTargetId} on the leaderboard in channel {ChannelId}",
                targetUser.UserId,
                channelId
            );

            Leaderboard leaderboard = await GetLeaderboardAsync(channelId, guildId);

            if (leaderboard.LeaderboardType != LeaderboardTypeEnum.Warning)
            {
                throw new CommandException(
                    $"The leaderboard in <#{channelId}> is not a warning leaderboard.",
                    showMessageToUser: true
                );
            }

            await AddScoreToLeaderboardAsync(
                targetUser.UserId,
                targetUser.UserId,
                leaderboard
            );
        }

        public async Task RegisterUserJoinedWithInviteAsync(
            DiscordGuildDto guild,
            DiscordUserDto userJoined,
            DiscordUserDto userInvitedBy
        )
        {
            if (
                (_hostEnvironment.IsProduction()
                    && await _discordRoleService.IsUserOwnerAsync(userInvitedBy.UserId, guild.GuildId))
                || await _discordRoleService.IsUserABot(userInvitedBy.UserId, guild.GuildId)
            )
            {
                _logger.Information(
                    "User {UserInvitedById} is either the owner or a bot in guild {GuildId}, skipping registration of user {UserJoinedId}",
                    userInvitedBy.UserId,
                    guild.GuildId,
                    userJoined.UserId
                );

                return;
            }

            (bool leaderboardExists, Leaderboard? leaderboard) =
                await _leaderboardRepository.DoesInviteLeaderboardExistOnServerAsync(
                    guild.GuildId
                );

            if (!leaderboardExists)
            {
                _logger.Information(
                    "No invite leaderboard exists in guild {GuildId}, skipping registration of user {UserJoinedId}",
                    guild.GuildId,
                    userJoined.UserId
                );
                return;
        }

        public async Task<bool> RemoveLeaderboardAsync(ulong channelId, ulong guildId)
        {
            _logger.Information(
                "Removing leaderboard in channel {ChannelId} for server {GuildId}",
                channelId,
                guildId
            );

            Leaderboard leaderboard = await GetLeaderboardAsync(channelId, guildId);

            var existingMessages = await _leaderboardMessageRepository.GetMessagesForLeaderboardAsync(leaderboard.Id);

            await _leaderboardRepository.RemoveLeaderboardAsync(channelId, guildId);

            await _messageService.DeleteMessagesForChannelAsync(existingMessages, channelId);

            var existingLeaderboards = await GetLeaderboardsForGuildAsync(guildId);
            existingLeaderboards.RemoveAll(l => l.Id == leaderboard.Id);

            _cacheService.Update<List<Leaderboard>>(
                CacheKeys.LeaderboardsPerGuild(guildId),
                _ => existingLeaderboards
            );
            return true;
        }

        public async Task<bool> UpdateLeaderboardMessageAsync(Leaderboard leaderboard, DiscordChannelDto channel)
        {
            var topUpvoteScores = await _leaderboardScoreRepository.GetTopScoresAsync(
                leaderboard.Id,
                TOP_UPVOTE_SCORES_LIMIT
            );

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

        public async Task<bool> UpvoteUserOnLeaderboardAsync(
            ulong channelId,
            ulong guildId,
            DiscordUserDto userUpvoting,
            DiscordUserDto targetUser
        )
        {
            _logger.Information(
                "User {UserUpvotingId} is upvoting user {UserTargetId} on the leaderboard in channel {ChannelId}",
                userUpvoting.UserId,
                targetUser.UserId,
                channelId
            );

            Leaderboard leaderboard = await GetLeaderboardAsync(channelId, guildId);

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

        private async Task<string> GenerateLeaderboardMessageAsync(
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

                try
                {
                    member = await _discordMemberService.GetMemberAsync(guildId, score.DiscordUserId);
                }
                catch (Exception ex)
                {
                    _logger.Warning(
                        ex,
                        "Failed to retrieve Discord member with ID {DiscordUserId} for leaderboard score with ID {ScoreId}. Trying to fetch User instead.",
                        score.DiscordUserId,
                        score.Id
                    );

                    try
                    {
                        member = await _discordUserService.GetUserAsync(score.DiscordUserId) as DiscordMemberDto;
                    }
                    catch (Exception ex2)
                    {
                        _logger.Warning(
                            ex2,
                            "Failed to retrieve Discord user with ID {DiscordUserId} for leaderboard score with ID {ScoreId}. Skipping this score.",
                            score.DiscordUserId,
                            score.Id
                        );
                        continue;
                    }
                }

                if (member is null)
                {
                    _logger.Warning(
                        "Discord member with ID {DiscordUserId} for leaderboard score with ID {ScoreId} could not be found. Skipping this score.",
                        score.DiscordUserId,
                        score.Id
                    );
                    continue;
                }

                string displayName = member.DisplayName ?? member.GlobalName ?? member.Username;
                sb.AppendLine($"{rank}. **{score.Score}** - **{displayName}** ({member.Username})");
                rank++;
            }

            return sb.ToString();
        }

        private async Task<Leaderboard> GetLeaderboardAsync(ulong channelId, ulong guildId)
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

        private async Task AddScoreToLeaderboardAsync(ulong userId, ulong targetUserId, Leaderboard leaderboard)
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

            await _leaderboardScoreSourceRepository.AddScoreAsync(userId, targetUserId, leaderboard.Id);
            await _leaderboardScoreRepository.IncrementScoreAsync(targetUserId, leaderboard.Id);

            LeaderboardUpdateJob leaderboardUpdateJob = new LeaderboardUpdateJob
            {
                Leaderboard = leaderboard,
                Channel = leaderboardChannel,
            };

            _leaderboardQueue.Enqueue(leaderboardUpdateJob);
        }
    }
}
