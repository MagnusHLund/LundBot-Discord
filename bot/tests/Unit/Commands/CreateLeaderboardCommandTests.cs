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
    public async Task CreateLeaderboardAsync_WhenCommandNotFromServer_DoesNothing()
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
    public async Task CreateLeaderboardAsync_WhenTitleInvalid_SendsValidationError()
    {
        // Arrange
        var interactionService = new MockDiscordInteractionService();
        var leaderboardService = new Mock<ILeaderboardService>();
        var command = new CreateLeaderboardCommand(leaderboardService.Object, interactionService);
        var channel = DiscordTestHelper.TestChannel(2000);

        // Act
        await command.CreateLeaderboardAsync(null!, channel, LeaderboardType.Upvote, "  ", "message");

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
}
