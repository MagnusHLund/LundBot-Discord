using System.Collections.Concurrent;
using LundBot.Application.Common.Exceptions;
using LundBot.Application.Common.Persistence;
using LundBot.Application.Discord.Channels;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Domain.Leaderboards;

namespace LundBot.Application.Features.Leaderboards.Shared
{
    public abstract class AbstractLeaderboardService : ILeaderboardService
    {
        private readonly ILeaderboardQueue _leaderboardQueue;
        private readonly ILeaderboardScoreRepository _leaderboardScoreRepository;
        private readonly ILeaderboardScoreSourceRepository _leaderboardScoreSourceRepository;
        private readonly IDiscordChannelService _discordChannelService;
        private readonly ILeaderboardService _leaderboardService;
        private readonly IUnitOfWork _unitOfWork;

        // There is no database constraint enforcing "one score per (leaderboard, actor, target)"
        // pair, because leaderboard types such as Warn intentionally allow the same actor to score
        // the same target multiple times. For leaderboard types that DO enforce a one-time score
        // (Upvote, Invite) via a "has already scored" pre-check, this lock serializes the
        // check-then-insert sequence per (leaderboard, actor, target) so two concurrent requests
        // can't both pass the check and award duplicate points.
        private static readonly ConcurrentDictionary<(int LeaderboardId, ulong ActorId, ulong TargetId), SemaphoreSlim> _scoreLocks =
            new();

        protected AbstractLeaderboardService(
            IDiscordChannelService discordChannelService,
            ILeaderboardQueue leaderboardQueue,
            ILeaderboardScoreRepository leaderboardScoreRepository,
            ILeaderboardScoreSourceRepository leaderboardScoreSourceRepository,
            ILeaderboardService leaderboardService,
            IUnitOfWork unitOfWork
        )
        {
            _leaderboardQueue = leaderboardQueue;
            _discordChannelService = discordChannelService;
            _leaderboardScoreRepository = leaderboardScoreRepository;
            _leaderboardScoreSourceRepository = leaderboardScoreSourceRepository;
            _leaderboardService = leaderboardService;
            _unitOfWork = unitOfWork;
        }

        public Task<bool> CreateLeaderboardAsync(
            ulong channelId,
            string title,
            string message,
            LeaderboardTypeEnum type
        ) => _leaderboardService.CreateLeaderboardAsync(channelId, title, message, type);

        public Task<bool> RemoveLeaderboardAsync(ulong channelId) =>
            _leaderboardService.RemoveLeaderboardAsync(channelId);

        public Task<bool> RefreshLeaderboardAsync(ulong channelId, ulong guildId) =>
            _leaderboardService.RefreshLeaderboardAsync(channelId, guildId);

        public Task<bool> UpdateLeaderboardMessageAsync(Leaderboard leaderboard, DiscordChannelDto channel) =>
            _leaderboardService.UpdateLeaderboardMessageAsync(leaderboard, channel);

        public ValueTask<List<Leaderboard>> GetLeaderboardsForGuildAsync(ulong guildId) =>
            _leaderboardService.GetLeaderboardsForGuildAsync(guildId);

        public Task<Leaderboard> GetLeaderboardAsync(ulong channelId, ulong guildId) =>
            _leaderboardService.GetLeaderboardAsync(channelId, guildId);

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

            bool addScoreResult = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                bool sourceAdded = await _leaderboardScoreSourceRepository.AddScoreAsync(
                    userId,
                    targetUserId,
                    leaderboard.Id
                );

                if (!sourceAdded)
                {
                    return false;
                }

                return await _leaderboardScoreRepository.IncrementScoreAsync(targetUserId, leaderboard.Id);
            });

            if (!addScoreResult)
            {
                throw new CommandException(
                    $"Failed to add score for user with ID {targetUserId} in leaderboard with ID {leaderboard.Id}.",
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

        /// <summary>
        /// Checks whether <paramref name="userId"/> has already scored <paramref name="targetUserId"/> on
        /// <paramref name="leaderboard"/> and, if not, adds the score. The check and the add are serialized
        /// per (leaderboard, actor, target) so two concurrent calls can't both observe "not yet scored" and
        /// both award a point - there is no database constraint to fall back on here because other
        /// leaderboard types (e.g. Warn) intentionally allow repeated scoring of the same target.
        /// </summary>
        /// <returns>False if the user has already scored the target, otherwise true.</returns>
        private protected async Task<bool> TryAddScoreOnceToLeaderboardAsync(
            ulong userId,
            ulong targetUserId,
            Leaderboard leaderboard
        )
        {
            var lockKey = (leaderboard.Id, userId, targetUserId);
            SemaphoreSlim scoreLock = _scoreLocks.GetOrAdd(lockKey, _ => new SemaphoreSlim(1, 1));
            await scoreLock.WaitAsync();

            try
            {
                bool hasAlreadyScored = await _leaderboardScoreSourceRepository.HasUserGivenScoreToTargetAsync(
                    userId,
                    targetUserId,
                    leaderboard.Id
                );

                if (hasAlreadyScored)
                {
                    return false;
                }

                await AddScoreToLeaderboardAsync(userId, targetUserId, leaderboard);
                return true;
            }
            finally
            {
                scoreLock.Release();
            }
        }
    }
}
