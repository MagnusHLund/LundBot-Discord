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
            IMessageService<
                LeaderboardMessagesEntity,
                LeaderboardMessagesRepository,
                LeaderboardMessageFactory
            > messageService
        )
        {
            _leaderboardsRepository = leaderboardsRepository;
            _leaderboardsMessageRepository = leaderboardsMessageRepository;
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

            var existingMessages =
                await _leaderboardsMessageRepository.GetMessagesForLeaderboardAsync(leaderboard.Id);

            await _leaderboardsRepository.RemoveLeaderboardAsync(
                channel.Id.ToString(),
                channel.Guild.Id.ToString()
            );

            await _messageService.DeleteMessagesForChannelAsync(existingMessages, channel);
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

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"**{message}**");
            sb.AppendLine($"# {title}");
            sb.AppendLine();
            sb.AppendLine("Empty leaderboard. Be the first to reach the top!");

            string leaderboardMessage = sb.ToString();

            await _messageService.SynchronizeDiscordMessagesAsync(
                leaderboardMessage,
                Enumerable.Empty<LeaderboardMessagesEntity>(),
                channel.Id
            );
        }
    }
}
