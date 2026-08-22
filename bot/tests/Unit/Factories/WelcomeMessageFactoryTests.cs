using LundBot.Factories.MessageEntityFactories;

namespace LundBot.Tests.Unit.Factories;

public sealed class WelcomeMessageFactoryTests
{
    [Fact]
    internal void Create_WhenSetJoinedUserIdNotCalled_ReturnsEntityWithEmptyUserId()
    {
        // Arrange
        var factory = new WelcomeMessageFactory();

        // Act
        var entity = factory.Create("msg-123");

        // Assert
        Assert.Equal("msg-123", entity.DiscordMessageId);
        Assert.Equal(string.Empty, entity.DiscordUserId);
    }

    [Fact]
    internal void Create_AfterSetJoinedUserId_ReturnsEntityWithCorrectUserId()
    {
        // Arrange
        var factory = new WelcomeMessageFactory();
        factory.SetJoinedUserId("42");

        // Act
        var entity = factory.Create("msg-456");

        // Assert
        Assert.Equal("msg-456", entity.DiscordMessageId);
        Assert.Equal("42", entity.DiscordUserId);
    }

    [Fact]
    internal void SetJoinedUserId_OverwritesPreviousValue()
    {
        // Arrange
        var factory = new WelcomeMessageFactory();
        factory.SetJoinedUserId("first");

        // Act
        factory.SetJoinedUserId("second");
        var entity = factory.Create("msg-789");

        // Assert
        Assert.Equal("second", entity.DiscordUserId);
    }

    [Fact]
    internal void Create_WithEmptyMessageId_ReturnsEntityWithEmptyDiscordMessageId()
    {
        // Arrange
        var factory = new WelcomeMessageFactory();
        factory.SetJoinedUserId("99");

        // Act
        var entity = factory.Create(string.Empty);

        // Assert
        Assert.Equal(string.Empty, entity.DiscordMessageId);
        Assert.Equal("99", entity.DiscordUserId);
    }
}
