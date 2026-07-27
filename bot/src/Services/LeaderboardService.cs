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
        private readonly ILeaderboardsRepository _leaderboardsRepository;
        private readonly IMessageService<
            LeaderboardMessagesEntity,
            LeaderboardMessagesRepository,
            LeaderboardMessageFactory
        > _messageService;

        public LeaderboardService(
            ILeaderboardsRepository leaderboardsRepository,
            IMessageService<
                LeaderboardMessagesEntity,
                LeaderboardMessagesRepository,
                LeaderboardMessageFactory
            > messageService
        )
        {
            _leaderboardsRepository = leaderboardsRepository;
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

        public async Task CreateInviteLeaderboardAsync()
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

        public async Task RemoveLeaderboardAsync()
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

        private async Task CreateLeaderboardAsync(
            DiscordChannel channel,
            string title,
            string message,
            LeaderboardType leaderboardType
        )
        {
            bool doesLeaderboardExist = await _leaderboardsRepository.DoesLeaderboardExistAsync(
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
