using LundBot.Helpers;

namespace LundBot.Tests.Unit.Helpers;

public sealed class CacheKeyHelperTests
{
    [Fact]
    public void LeaderboardsPerGuild_WhenGuildIdProvided_ReturnsExpectedKey()
    {
        // Arrange
        const string guildId = "123";

        // Act
        string key = CacheKeyHelper.LeaderboardsPerGuild(guildId);

        // Assert
        Assert.Equal("guild_leaderboards_123", key);
    }

    [Fact]
    public void GuildInvites_WhenGuildIdProvided_ReturnsExpectedKey()
    {
        // Arrange
        const string guildId = "456";

        // Act
        string key = CacheKeyHelper.GuildInvites(guildId);

        // Assert
        Assert.Equal("guild_invites_456", key);
    }
}
