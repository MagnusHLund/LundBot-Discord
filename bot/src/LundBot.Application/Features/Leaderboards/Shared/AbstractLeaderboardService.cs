using LundBot.Application.Common.Exceptions;
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

        protected AbstractLeaderboardService(
            IDiscordChannelService discordChannelService,
            ILeaderboardQueue leaderboardQueue,
            ILeaderboardScoreRepository leaderboardScoreRepository,
            ILeaderboardScoreSourceRepository leaderboardScoreSourceRepository,
            ILeaderboardService leaderboardService
        )
        {
            _leaderboardQueue = leaderboardQueue;
            _discordChannelService = discordChannelService;
            _leaderboardScoreRepository = leaderboardScoreRepository;
            _leaderboardScoreSourceRepository = leaderboardScoreSourceRepository;
            _leaderboardService = leaderboardService;
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

            bool addScoreResult = await _leaderboardScoreSourceRepository.AddScoreAsync(
                userId,
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

            bool incrementScoreResult = await _leaderboardScoreRepository.IncrementScoreAsync(
                targetUserId,
                leaderboard.Id
            );

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
