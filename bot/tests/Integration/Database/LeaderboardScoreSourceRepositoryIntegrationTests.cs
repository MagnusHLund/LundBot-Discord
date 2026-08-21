using LundBot.Entities;
using LundBot.Enums;
using LundBot.Repositories;
using LundBot.Tests.Fixtures.Data;

namespace LundBot.Tests.Integration.Database;

public sealed class LeaderboardScoreSourceRepositoryIntegrationTests
{
    [Fact]
    internal async Task AddScoreAsync_WhenInserted_CanBeFoundByHasUserGivenScoreToTargetAsync()
    {
        // Arrange
        using var fixture = new SqliteDbFixture();
        var leaderboardsRepository = new LeaderboardsRepository(fixture.Db);
        var repository = new LeaderboardScoreSourceRepository(fixture.Db);
        LeaderboardsEntity leaderboard = await leaderboardsRepository.CreateLeaderboardAsync(
            "123",
            "456",
            "Title",
            "Message",
            LeaderboardType.Upvote
        );

        // Act
        await repository.AddScoreAsync("actor", "target", leaderboard.Id);
        bool exists = await repository.HasUserGivenScoreToTargetAsync(
            "actor",
            "target",
            leaderboard.Id
        );

        // Assert
        Assert.True(exists);
    }
}
