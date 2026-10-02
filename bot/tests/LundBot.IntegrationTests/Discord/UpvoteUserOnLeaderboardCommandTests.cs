using DSharpPlus.Commands;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Presentation.Discord.Leaderboards.Commands;
using NSubstitute;
using Xunit;

namespace LundBot.IntegrationTests.Discord;

public sealed class UpvoteUserOnLeaderboardCommandTests
{
    [Fact]
    public async Task Upvote_stops_when_used_outside_a_guild()
    {
        CommandContext context = DiscordCommandTestDoubles.CreateContext();
        var interactionService = DiscordCommandTestDoubles.CreateInteractionService(isGuild: false);
        IUpvoteLeaderboardService leaderboardService = Substitute.For<IUpvoteLeaderboardService>();
        UpvoteUserOnLeaderboardCommand command = new UpvoteUserOnLeaderboardCommand(
            interactionService,
            leaderboardService
        );

        await command.UpvoteUserAsync(context, 0, null!);

        await interactionService.Received(1).IsCommandSentFromGuild(Arg.Any<CommandContext>());
        await leaderboardService.DidNotReceiveWithAnyArgs().UpvoteUserAsync(default, default!, default!);
    }
}
