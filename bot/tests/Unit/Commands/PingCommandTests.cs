using LundBot.Commands;
using LundBot.Tests.Mocks.Services.Discord;

namespace LundBot.Tests.Unit.Commands;

public sealed class PingCommandTests
{
    [Fact]
    internal async Task PingAsync_WhenCalled_SendsPongResponse()
    {
        // Arrange
        var interactionService = new MockDiscordInteractionService();
        var command = new PingCommand(interactionService);

        // Act
        await command.PingAsync(null!);

        // Assert
        Assert.Single(interactionService.Responses);
        Assert.Equal("Pong!", interactionService.Responses[0].Content);
        Assert.True(interactionService.Responses[0].ShowOnlyToUser);
    }
}
