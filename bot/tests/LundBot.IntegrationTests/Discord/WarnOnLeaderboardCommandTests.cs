using DSharpPlus.Commands;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Presentation.Discord.Leaderboards.Commands;
using NSubstitute;
using Xunit;

namespace LundBot.IntegrationTests.Discord;

public sealed class WarnOnLeaderboardCommandTests
{
    [Fact]
    public async Task Warn_stops_when_used_outside_a_guild()
    {
        CommandContext context = DiscordCommandTestDoubles.CreateContext();
        var interactionService = DiscordCommandTestDoubles.CreateInteractionService(isGuild: false);
        IWarnLeaderboardService leaderboardService = Substitute.For<IWarnLeaderboardService>();
        WarnOnLeaderboardCommand command = new WarnOnLeaderboardCommand(leaderboardService, interactionService);

        await command.RegisterWarningAsync(context, 0, null!);

        await interactionService.Received(1).IsCommandSentFromGuild(Arg.Any<CommandContext>());
        await leaderboardService.DidNotReceiveWithAnyArgs().RegisterWarningAsync(default, default, default);
    }
}
