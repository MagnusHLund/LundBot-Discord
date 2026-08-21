using LundBot.Entities;
using LundBot.Enums;
using LundBot.Repositories;
using LundBot.Tests.Fixtures.Data;

namespace LundBot.Tests.Integration.Database;

public sealed class LeaderboardsRepositoryIntegrationTests
{
    [Fact]
    internal async Task CreateAndFindLeaderboardAsync_WhenPersisted_ReturnsStoredLeaderboard()
    {
        // Arrange
        using var fixture = new SqliteDbFixture();
        var repository = new LeaderboardsRepository(fixture.Db);

        // Act
        LeaderboardsEntity created = await repository.CreateLeaderboardAsync(
            "100",
            "200",
            "Top users",
            "Weekly",
            LeaderboardType.Upvote
        );
        (bool exists, LeaderboardsEntity? found) = await repository.DoesLeaderboardExistAsync(
            "100",
            "200"
        );

        // Assert
        Assert.True(exists);
        Assert.NotNull(found);
        Assert.Equal(created.Id, found!.Id);
        Assert.Equal("Top users", found.Title);
    }

    [Fact]
    internal async Task RemoveLeaderboardAsync_WhenPersisted_RemovesLeaderboard()
    {
        // Arrange
        using var fixture = new SqliteDbFixture();
        var repository = new LeaderboardsRepository(fixture.Db);
        await repository.CreateLeaderboardAsync(
            "300",
            "400",
            "Title",
            "Message",
            LeaderboardType.Warning
        );

        // Act
        await repository.RemoveLeaderboardAsync("300", "400");
        (bool exists, _) = await repository.DoesLeaderboardExistAsync("300", "400");

        // Assert
        Assert.False(exists);
    }
}
