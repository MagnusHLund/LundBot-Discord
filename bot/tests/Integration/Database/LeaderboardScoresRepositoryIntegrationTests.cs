using LundBot.Entities;
using LundBot.Enums;
using LundBot.Repositories;
using LundBot.Tests.Fixtures.Data;

namespace LundBot.Tests.Integration.Database;

public sealed class LeaderboardScoresRepositoryIntegrationTests
{
    [Fact]
    internal async Task IncrementScoreAsync_WhenSameUserIncrementedTwice_StoresScoreOfTwo()
    {
        // Arrange
        using var fixture = new SqliteDbFixture();
        var leaderboardsRepository = new LeaderboardsRepository(fixture.Db);
        var scoresRepository = new LeaderboardScoresRepository(fixture.Db);
        LeaderboardsEntity leaderboard = await leaderboardsRepository.CreateLeaderboardAsync(
            "1000",
            "2000",
            "Title",
            "Message",
            LeaderboardType.Upvote
        );

        // Act
        await scoresRepository.IncrementScoreAsync("user-1", leaderboard.Id);
        await scoresRepository.IncrementScoreAsync("user-1", leaderboard.Id);
        IEnumerable<LeaderboardScoresEntity> scores = await scoresRepository.GetTopScoresAsync(
            leaderboard.Id,
            10
        );

        // Assert
        LeaderboardScoresEntity score = Assert.Single(scores);
        Assert.Equal(2, score.Score);
        Assert.Equal("user-1", score.DiscordUserId);
    }
}
