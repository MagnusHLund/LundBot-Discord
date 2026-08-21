using LundBot.Commands;
using LundBot.Tests.Mocks.Services.Discord;

namespace LundBot.Tests.Unit.Commands;

public sealed class RandomMapCommandTests
{
    [Fact]
    public async Task RandomMapAsync_WhenCalled_SendsOneMapName()
    {
        // Arrange
        var interactionService = new MockDiscordInteractionService();
        var command = new RandomMapCommand(interactionService);

        // Act
        await command.RandomMapAsync(null!);

        // Assert
        Assert.Single(interactionService.Responses);
        Assert.False(string.IsNullOrWhiteSpace(interactionService.Responses[0].Content));
    }
}
