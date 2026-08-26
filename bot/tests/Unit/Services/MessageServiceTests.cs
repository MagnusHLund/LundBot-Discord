using DSharpPlus.Entities;
using LundBot.Entities;
using LundBot.Tests.Mocks.Repositories;
using LundBot.Tests.Mocks.Services.Discord;
using LundBot.Tests.TestHelpers;
using LundBot.Tests.Unit.Services.Factories;

namespace LundBot.Tests.Unit.Services;

public sealed class MessageServiceTests
{
    [Fact]
    internal async Task SynchronizeDiscordMessagesAsync_UpdatesExistingMessages()
    {
        // Arrange
        var repo = new MockMessageRepository<LeaderboardMessagesEntity>();
        var discord = new MockDiscordMessageService();
        var channelService = new MockDiscordChannelService();

        discord.GetMessageBehavior = id =>
            DiscordTestHelper.TestMessage(id, channelService.Channel);

        var existing = new List<LeaderboardMessagesEntity>
        {
            new() { Id = 1, DiscordMessageId = "10" },
        };

        var service = MessageServiceTestFactory.Create(discord, channelService, repo);

        // Act
        await service.SynchronizeDiscordMessagesAsync(
            "Updated text",
            existing,
            channelService.Channel.Id
        );

        // Assert
        Assert.Single(discord.Modified);
        Assert.Equal("Updated text", discord.Modified[0].NewContent);
    }

    [Fact]
    internal async Task SynchronizeDiscordMessagesAsync_CreatesNewMessages()
    {
        // Arrange
        var repo = new MockMessageRepository<LeaderboardMessagesEntity>();
        var discord = new MockDiscordMessageService();
        var channelService = new MockDiscordChannelService();

        discord.GetMessageBehavior = id =>
            DiscordTestHelper.TestMessage(id, channelService.Channel);

        var existing = new List<LeaderboardMessagesEntity>();

        var service = MessageServiceTestFactory.Create(discord, channelService, repo);

        // Act
        await service.SynchronizeDiscordMessagesAsync(
            "Hello world",
            existing,
            channelService.Channel.Id
        );

        // Assert
        Assert.Single(discord.Sent);
        Assert.Single(repo.Created);
    }

    [Fact]
    internal async Task SynchronizeDiscordMessagesAsync_DeletesExtraMessages()
    {
        // Arrange
        var repo = new MockMessageRepository<LeaderboardMessagesEntity>();
        var discord = new MockDiscordMessageService();
        var channelService = new MockDiscordChannelService();

        discord.GetMessageBehavior = id =>
            DiscordTestHelper.TestMessage(id, channelService.Channel);

        var existing = new List<LeaderboardMessagesEntity>
        {
            new() { Id = 1, DiscordMessageId = "10" },
            new() { Id = 2, DiscordMessageId = "11" },
        };

        var service = MessageServiceTestFactory.Create(discord, channelService, repo);

        // Act
        await service.SynchronizeDiscordMessagesAsync(
            "Only one chunk",
            existing,
            channelService.Channel.Id
        );

        // Assert
        Assert.Single(discord.Deleted);
        Assert.Single(repo.Deleted);
        Assert.Equal(2, repo.Deleted[0]);
    }

    [Fact]
    internal async Task DeleteMessageByIdAsync_FetchesMessageFromDiscordAndDeletesFromDiscordAndRepository()
    {
        // Arrange
        var repo = new MockMessageRepository<LeaderboardMessagesEntity>();
        var discord = new MockDiscordMessageService();
        var channelService = new MockDiscordChannelService();

        var entity = new LeaderboardMessagesEntity { Id = 5, DiscordMessageId = "999" };
        var expectedMessage = DiscordTestHelper.TestMessage(999, channelService.Channel);
        discord.GetMessageBehavior = _ => expectedMessage;

        var service = MessageServiceTestFactory.Create(discord, channelService, repo);

        // Act
        await service.DeleteMessageByIdAsync(entity, channelService.Channel);

        // Assert
        Assert.Single(discord.Deleted);
        Assert.Contains(5, repo.Deleted);
    }

    [Fact]
    internal async Task CreateMessageWithComponentsAsync_SendsWithComponentsAndCreatesEntityInRepository()
    {
        // Arrange
        var repo = new MockMessageRepository<LeaderboardMessagesEntity>();
        var discord = new MockDiscordMessageService();
        var channelService = new MockDiscordChannelService();

        var components = new List<DiscordComponent>
        {
            new DiscordButtonComponent(DiscordButtonStyle.Primary, "test_id", "Test"),
        };

        var service = MessageServiceTestFactory.Create(discord, channelService, repo);

        // Act
        await service.CreateMessageWithComponentsAsync(
            "Hello!",
            channelService.Channel,
            components
        );

        // Assert
        Assert.Single(discord.SentWithComponents);
        Assert.Equal("Hello!", discord.SentWithComponents[0].Content);
        Assert.Single(repo.Created);
    }
}
