using DSharpPlus.Commands;
using LundBot.Presentation.Discord.Interactions;
using NSubstitute;

namespace LundBot.IntegrationTests.Discord;

internal static class DiscordCommandTestDoubles
{
    public static CommandContext CreateContext() => Substitute.For<CommandContext>();

    public static IDiscordInteractionService CreateInteractionService(bool isGuild = true)
    {
        IDiscordInteractionService interactionService = Substitute.For<IDiscordInteractionService>();
        interactionService.IsCommandSentFromGuild(Arg.Any<CommandContext>()).Returns(new ValueTask<bool>(isGuild));
        return interactionService;
    }
}
