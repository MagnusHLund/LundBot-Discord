using DSharpPlus.Commands;
using LundBot.Presentation.Discord.Bot;
using NSubstitute;
using Xunit;

namespace LundBot.IntegrationTests.Discord;

public sealed class PingCommandTests
{
    [Fact]
    public async Task Ping_sends_pong_response()
    {
        CommandContext context = DiscordCommandTestDoubles.CreateContext();
        var interactionService = DiscordCommandTestDoubles.CreateInteractionService();
        interactionService.SendResponseAsync(Arg.Any<CommandContext>(), "Pong!", true).Returns(Task.FromResult(true));
        PingCommand command = new PingCommand(interactionService);

        await command.PingAsync(context);

        await interactionService.Received(1).SendResponseAsync(Arg.Any<CommandContext>(), "Pong!", true);
    }
}
