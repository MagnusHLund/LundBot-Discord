using LundBot.Entities;
using LundBot.Enums;
using LundBot.Factories.MessageEntityFactories;
using LundBot.Helpers;
using LundBot.Interfaces.Queues;
using LundBot.Interfaces.Repositories;
using LundBot.Interfaces.Services;
using LundBot.Interfaces.Services.Discord;
using LundBot.Repositories;
using LundBot.Services;
using LundBot.Tests.TestHelpers;
using LundBot.ValueObjects.Jobs;
using Moq;

namespace LundBot.Tests.Unit.Services;

public sealed class LeaderboardServiceTests
{
    [Fact]
    public async Task RegisterUserJoinedWithInviteAsync_WhenNoInviteLeaderboardExists_DoesNotWriteScore()
    {
        // Arrange
        var context = CreateContext();
        var guild = DiscordObjectFactory.CreateUninitializedGuild(10);
        var joinedUser = DiscordTestHelper.TestUser(100, "Joined");
        var inviter = DiscordTestHelper.TestUser(200, "Inviter");

        context.LeaderboardsRepository
            .Setup(r => r.DoesInviteLeaderboardExistOnServerAsync("10"))
            .ReturnsAsync((false, null));

        // Act
        await context.Service.RegisterUserJoinedWithInviteAsync(guild, joinedUser, inviter);

        // Assert
        context.LeaderboardScoreSourceRepository.Verify(
            r => r.AddScoreAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
            Times.Never
        );
        context.LeaderboardScoresRepository.Verify(
            r => r.IncrementScoreAsync(It.IsAny<string>(), It.IsAny<int>()),
            Times.Never
        );
    }

    [Fact]
    public async Task RegisterUserJoinedWithInviteAsync_WhenValidInvite_AddsScoreAndEnqueuesJob()
    {
        // Arrange
        var context = CreateContext();
        var guild = DiscordObjectFactory.CreateUninitializedGuild(14);
        var joinedUser = DiscordTestHelper.TestUser(301, "Joined");
        var inviter = DiscordTestHelper.TestUser(401, "Inviter");
        var inviteChannel = DiscordTestHelper.TestChannel(2222);
        LeaderboardsEntity leaderboard =
            new()
            {
                Id = 33,
                DiscordServerId = "14",
                DiscordChannelId = "2222",
                Title = "Invites",
                Message = "Invite leaderboard",
                LeaderboardType = LeaderboardType.Invite,
            };

        context.LeaderboardsRepository
            .Setup(r => r.DoesInviteLeaderboardExistOnServerAsync("14"))
            .ReturnsAsync((true, leaderboard));
        context.LeaderboardScoreSourceRepository
            .Setup(r => r.HasUserGivenScoreToTargetAsync("401", "301", 33))
            .ReturnsAsync(false);
        context.DiscordChannelService.Setup(s => s.GetChannelAsync(2222)).ReturnsAsync(inviteChannel);

        LeaderboardUpdateJob? queuedJob = null;
        context.LeaderboardQueue
            .Setup(q => q.Enqueue(It.IsAny<LeaderboardUpdateJob>()))
            .Callback<LeaderboardUpdateJob>(job => queuedJob = job);

        // Act
        await context.Service.RegisterUserJoinedWithInviteAsync(guild, joinedUser, inviter);

        // Assert
        context.LeaderboardScoreSourceRepository.Verify(r => r.AddScoreAsync("301", "401", 33), Times.Once);
        context.LeaderboardScoresRepository.Verify(r => r.IncrementScoreAsync("401", 33), Times.Once);
        Assert.NotNull(queuedJob);
        Assert.Equal(33, queuedJob!.Leaderboard.Id);
    }

    [Fact]
    public async Task GetLeaderboardsForGuildAsync_WhenCacheHasValue_ReturnsCache()
    {
        // Arrange
        var context = CreateContext();
        var cached = new List<LeaderboardsEntity> { new() { Id = 1, DiscordServerId = "500", DiscordChannelId = "700", Title = "T1", Message = "M1" } };
        context.CacheService
            .Setup(c => c.Get<List<LeaderboardsEntity>>(CacheKeyHelper.LeaderboardsPerGuild("500")))
            .Returns(cached);

        // Act
        List<LeaderboardsEntity> result = await context.Service.GetLeaderboardsForGuildAsync("500");

        // Assert
        Assert.Single(result);
        context.LeaderboardsRepository.Verify(r => r.GetLeaderboardsForGuildAsync("500"), Times.Never);
    }

    private static LeaderboardServiceTestContext CreateContext()
    {
        var userService = new Mock<IUserService>();
        userService.Setup(s => s.IsUserOwnerAsync(It.IsAny<ulong>(), It.IsAny<ulong>())).ReturnsAsync(false);
        userService.Setup(s => s.IsUserABot(It.IsAny<ulong>(), It.IsAny<ulong>())).ReturnsAsync(false);

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
            service,
            leaderboardsRepository,
            leaderboardScoreSourceRepository,
            leaderboardScoresRepository,
            cacheService,
            discordChannelService,
            leaderboardQueue
        );
    }

    private sealed record LeaderboardServiceTestContext(
        LeaderboardService Service,
        Mock<ILeaderboardsRepository> LeaderboardsRepository,
        Mock<ILeaderboardScoreSourceRepository> LeaderboardScoreSourceRepository,
        Mock<ILeaderboardScoresRepository> LeaderboardScoresRepository,
        Mock<ICacheService> CacheService,
        Mock<IDiscordChannelService> DiscordChannelService,
        Mock<ILeaderboardQueue> LeaderboardQueue
    );
}
