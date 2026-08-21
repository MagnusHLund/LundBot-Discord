using LundBot.Commands;
using LundBot.Enums;
using LundBot.Interfaces.Services;
using LundBot.Tests.Mocks.Services.Discord;
using LundBot.Tests.TestHelpers;
using Moq;

namespace LundBot.Tests.Integration.Discord;

public sealed class CreateLeaderboardCommandIntegrationTests
{
    [Fact]
    internal async Task CreateLeaderboardAsync_WhenValidInput_CallsServiceAndSendsSuccessMessage()
    {
        // Arrange
        var interactionService = new MockDiscordInteractionService();
        var leaderboardService = new Mock<ILeaderboardService>();
        var channel = DiscordTestHelper.TestChannel(5678);
        var command = new CreateLeaderboardCommand(leaderboardService.Object, interactionService);

        // Act
        await command.CreateLeaderboardAsync(
            null!,
            channel,
            LeaderboardType.Warning,
            "  Title  ",
            "  Message  "
        );

        // Assert
        leaderboardService.Verify(
            s => s.CreateLeaderboardAsync(channel, "Title", "Message", LeaderboardType.Warning),
            Times.Once
        );
        Assert.Single(interactionService.Responses);
        Assert.Contains("created successfully", interactionService.Responses[0].Content);
    }
}
