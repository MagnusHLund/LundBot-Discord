using LundBot.Entities;
using LundBot.Factories.MessageEntityFactories;
using LundBot.Interfaces.Queues;
using LundBot.Interfaces.Repositories;
using LundBot.Interfaces.Services;
using LundBot.Interfaces.Services.Discord;
using LundBot.Repositories;
using LundBot.Services;
using Moq;

namespace LundBot.Tests.Unit.Services.Contexts
{
    internal static class LeaderboardServiceTestContextFactory
    {
        internal static LeaderboardServiceTestContext Create()
        {
            var userService = new Mock<IUserService>();
            userService
                .Setup(s => s.IsUserOwnerAsync(It.IsAny<ulong>(), It.IsAny<ulong>()))
                .ReturnsAsync(false);
            userService
                .Setup(s => s.IsUserABot(It.IsAny<ulong>(), It.IsAny<ulong>()))
                .ReturnsAsync(false);

            var leaderboardsRepository = new Mock<ILeaderboardsRepository>();
            var leaderboardMessagesRepository = new Mock<ILeaderboardMessagesRepository>();
            var leaderboardScoreSourceRepository = new Mock<ILeaderboardScoreSourceRepository>();
            var leaderboardScoresRepository = new Mock<ILeaderboardScoresRepository>();
            var discordMemberService = new Mock<IDiscordMemberService>();
            var cacheService = new Mock<ICacheService>();
            var discordChannelService = new Mock<IDiscordChannelService>();
            var leaderboardQueue = new Mock<ILeaderboardQueue>();
            var messageService =
                new Mock<
                    IMessageService<
                        LeaderboardMessagesEntity,
                        LeaderboardMessagesRepository,
                        LeaderboardMessageFactory
                    >
                >();

            messageService.SetupGet(m => m.MessageFactory).Returns(new LeaderboardMessageFactory());

            var service = new LeaderboardService(
                userService.Object,
                leaderboardsRepository.Object,
                leaderboardMessagesRepository.Object,
                leaderboardScoreSourceRepository.Object,
                leaderboardScoresRepository.Object,
                discordMemberService.Object,
                cacheService.Object,
                discordChannelService.Object,
                messageService.Object,
                leaderboardQueue.Object
            );

            return new LeaderboardServiceTestContext(
                userService,
                service,
                leaderboardsRepository,
                leaderboardMessagesRepository,
                leaderboardScoreSourceRepository,
                leaderboardScoresRepository,
                cacheService,
                discordChannelService,
                leaderboardQueue
            );
        }
    }
}
