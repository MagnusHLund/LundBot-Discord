using DSharpPlus.Commands;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Presentation.Discord.Leaderboards.Commands;
using NSubstitute;
using Xunit;

namespace LundBot.IntegrationTests.Discord.Commands;

public sealed class CreateLeaderboardCommandTests
{
    [Fact]
    public async Task CreateLeaderboard_stops_when_used_outside_a_guild()
    {
        CommandContext context = DiscordCommandTestDoubles.CreateContext();
        var interactionService = DiscordCommandTestDoubles.CreateInteractionService(isGuild: false);
        ILeaderboardService leaderboardService = Substitute.For<ILeaderboardService>();
        CreateLeaderboardCommand command = new CreateLeaderboardCommand(interactionService, leaderboardService);

        await command.CreateLeaderboardAsync(context, null!, default, null!, null);

        await interactionService.Received(1).IsCommandSentFromGuild(Arg.Any<CommandContext>());
        await leaderboardService
            .DidNotReceiveWithAnyArgs()
            .CreateLeaderboardAsync(default, default!, default!, default);
    }
}
