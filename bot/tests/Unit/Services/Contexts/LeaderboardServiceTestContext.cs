using LundBot.Interfaces.Queues;
using LundBot.Interfaces.Repositories;
using LundBot.Interfaces.Services;
using LundBot.Interfaces.Services.Discord;
using LundBot.Services;
using Moq;

namespace LundBot.Tests.Unit.Services.Contexts;

internal sealed record LeaderboardServiceTestContext(
    Mock<IUserService> UserService,
    LeaderboardService Service,
    Mock<ILeaderboardsRepository> LeaderboardsRepository,
    Mock<ILeaderboardMessagesRepository> LeaderboardMessagesRepository,
    Mock<ILeaderboardScoreSourceRepository> LeaderboardScoreSourceRepository,
    Mock<ILeaderboardScoresRepository> LeaderboardScoresRepository,
    Mock<ICacheService> CacheService,
    Mock<IDiscordChannelService> DiscordChannelService,
    Mock<ILeaderboardQueue> LeaderboardQueue
);
