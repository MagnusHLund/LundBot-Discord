using LundBot.Commands;
using LundBot.Enums;
using LundBot.Interfaces.Services;
using LundBot.Tests.Mocks.Services.Discord;
using LundBot.Tests.TestHelpers;
using Moq;

namespace LundBot.Tests.Unit.Commands;

public sealed class CreateLeaderboardCommandTests
{
    [Fact]
    internal async Task CreateLeaderboardAsync_WhenCommandNotFromServer_DoesNothing()
    {
        // Arrange
        var interactionService = new MockDiscordInteractionService
        {
            IsCommandSentFromServerResult = false,
        };
        var leaderboardService = new Mock<ILeaderboardService>();
        var command = new CreateLeaderboardCommand(leaderboardService.Object, interactionService);
        var channel = DiscordTestHelper.TestChannel(1000);

        // Act
        await command.CreateLeaderboardAsync(
            null!,
            channel,
            LeaderboardType.Upvote,
            "Top users",
            "Weekly results"
        );

        // Assert
        leaderboardService.Verify(
            s =>
                s.CreateLeaderboardAsync(
                    It.IsAny<DSharpPlus.Entities.DiscordChannel>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<LeaderboardType>()
                ),
            Times.Never
        );
        Assert.Empty(interactionService.Responses);
    }

    [Fact]
    internal async Task CreateLeaderboardAsync_WhenTitleInvalid_SendsValidationError()
    {
        // Arrange
        var interactionService = new MockDiscordInteractionService();
        var leaderboardService = new Mock<ILeaderboardService>();
        var command = new CreateLeaderboardCommand(leaderboardService.Object, interactionService);
        var channel = DiscordTestHelper.TestChannel(2000);

        // Act
        await command.CreateLeaderboardAsync(
            null!,
            channel,
            LeaderboardType.Upvote,
            "  ",
            "message"
        );

        // Assert
        Assert.Single(interactionService.Responses);
        Assert.Equal(
            "The title must be between 1 and 64 characters long.",
            interactionService.Responses[0].Content
        );
        leaderboardService.Verify(
            s =>
                s.CreateLeaderboardAsync(
                    It.IsAny<DSharpPlus.Entities.DiscordChannel>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<LeaderboardType>()
                ),
            Times.Never
        );
    }

    [Fact]
    internal async Task CreateLeaderboardAsync_WhenMessageLengthIsInvalid_SendsValidationError()
    {
        // Arrange
        var interactionService = new MockDiscordInteractionService();
        var leaderboardService = new Mock<ILeaderboardService>();
        var command = new CreateLeaderboardCommand(leaderboardService.Object, interactionService);
        var channel = DiscordTestHelper.TestChannel(2001);
        var invalidMessage = new string('x', 257);

        // Act
        await command.CreateLeaderboardAsync(
            null!,
            channel,
            LeaderboardType.Warning,
            "Valid title",
            invalidMessage
        );

        // Assert
        Assert.Single(interactionService.Responses);
        Assert.Equal(
            "The message must be between 0 and 256 characters long.",
            interactionService.Responses[0].Content
        );
        leaderboardService.Verify(
            s =>
                s.CreateLeaderboardAsync(
                    It.IsAny<DSharpPlus.Entities.DiscordChannel>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<LeaderboardType>()
                ),
            Times.Never
        );
    }

    [Fact]
    internal async Task CreateLeaderboardAsync_WhenValidInput_InvokesServiceWithTrimmedValues()
    {
        // Arrange
        var interactionService = new MockDiscordInteractionService();
        var leaderboardService = new Mock<ILeaderboardService>();
        var command = new CreateLeaderboardCommand(leaderboardService.Object, interactionService);
        var channel = DiscordTestHelper.TestChannel(2002);

        // Act
        await command.CreateLeaderboardAsync(
            null!,
            channel,
            LeaderboardType.Upvote,
            "  Top users  ",
            "  Weekly results  "
        );

        // Assert
        leaderboardService.Verify(
            s =>
                s.CreateLeaderboardAsync(
                    channel,
                    "Top users",
                    "Weekly results",
                    LeaderboardType.Upvote
                ),
            Times.Once
        );
        Assert.Single(interactionService.Responses);
        Assert.Equal(
            "Upvote Leaderboard created successfully, in <#2002>.",
            interactionService.Responses[0].Content
        );
    }
}
