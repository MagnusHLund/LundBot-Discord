using LundBot.Commands;
using LundBot.Interfaces.Services;
using LundBot.Tests.Mocks.Services.Discord;
using LundBot.Tests.TestHelpers;
using Moq;

namespace LundBot.Tests.Unit.Commands;

public sealed class RemoveLeaderboardCommandTests
{
    [Fact]
    public async Task RemoveLeaderboardAsync_WhenConfirmFalse_SendsWarningAndSkipsServiceCall()
    {
        // Arrange
        var interactionService = new MockDiscordInteractionService();
        var leaderboardService = new Mock<ILeaderboardService>();
        var command = new RemoveLeaderboardCommand(leaderboardService.Object, interactionService);
        var channel = DiscordTestHelper.TestChannel(3333);

        // Act
        await command.RemoveLeaderboardAsync(null!, channel, confirm: false);

        // Assert
        Assert.Single(interactionService.Responses);
        Assert.Equal(
            "You must confirm the removal of the leaderboard by setting the 'Confirm' option to true.",
            interactionService.Responses[0].Content
        );
        leaderboardService.Verify(
            s => s.RemoveLeaderboardAsync(It.IsAny<DSharpPlus.Entities.DiscordChannel>()),
            Times.Never
        );
    }
}
