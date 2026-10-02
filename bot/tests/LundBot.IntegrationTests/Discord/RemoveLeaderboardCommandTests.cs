using DSharpPlus.Commands;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Presentation.Discord.Leaderboards.Commands;
using NSubstitute;
using Xunit;

namespace LundBot.IntegrationTests.Discord;

public sealed class RemoveLeaderboardCommandTests
{
    private const ulong ChannelId = 1531717575338102914;

    [Fact]
    public async Task RemoveLeaderboard_stops_when_used_outside_a_guild()
    {
        CommandContext context = DiscordCommandTestDoubles.CreateContext();
        var interactionService = DiscordCommandTestDoubles.CreateInteractionService(isGuild: false);
        ILeaderboardService leaderboardService = Substitute.For<ILeaderboardService>();
        RemoveLeaderboardCommand command = new RemoveLeaderboardCommand(interactionService, leaderboardService);

        await command.RemoveLeaderboardAsync(context, ChannelId, confirm: true);

        await interactionService.Received(1).IsCommandSentFromGuild(Arg.Any<CommandContext>());
        await leaderboardService.DidNotReceiveWithAnyArgs().RemoveLeaderboardAsync(default);
    }

    [Fact]
    public async Task RemoveLeaderboard_requires_confirmation()
    {
        CommandContext context = DiscordCommandTestDoubles.CreateContext();
        var interactionService = DiscordCommandTestDoubles.CreateInteractionService();
        ILeaderboardService leaderboardService = Substitute.For<ILeaderboardService>();
        RemoveLeaderboardCommand command = new RemoveLeaderboardCommand(interactionService, leaderboardService);

        await command.RemoveLeaderboardAsync(context, ChannelId, confirm: false);

        await interactionService
            .Received(1)
            .SendResponseAsync(
                Arg.Any<CommandContext>(),
                "You must confirm the removal of the leaderboard by setting the 'Confirm' option to true.",
                true
            );
        await leaderboardService.DidNotReceiveWithAnyArgs().RemoveLeaderboardAsync(default);
    }

    [Fact]
    public async Task RemoveLeaderboard_reports_success()
    {
        CommandContext context = DiscordCommandTestDoubles.CreateContext();
        var interactionService = DiscordCommandTestDoubles.CreateInteractionService();
        interactionService
            .SendResponseAsync(Arg.Any<CommandContext>(), Arg.Any<string>(), true)
            .Returns(Task.FromResult(true));
        ILeaderboardService leaderboardService = Substitute.For<ILeaderboardService>();
        leaderboardService.RemoveLeaderboardAsync(ChannelId).Returns(true);
        RemoveLeaderboardCommand command = new RemoveLeaderboardCommand(interactionService, leaderboardService);

        await command.RemoveLeaderboardAsync(context, ChannelId, confirm: true);

        await interactionService
            .Received(1)
            .SendResponseAsync(
                Arg.Any<CommandContext>(),
                $"Leaderboard removed successfully from <#{ChannelId}>.",
                true
            );
    }

    [Fact]
    public async Task RemoveLeaderboard_reports_false_service_result()
    {
        CommandContext context = DiscordCommandTestDoubles.CreateContext();
        var interactionService = DiscordCommandTestDoubles.CreateInteractionService();
        ILeaderboardService leaderboardService = Substitute.For<ILeaderboardService>();
        leaderboardService.RemoveLeaderboardAsync(ChannelId).Returns(false);
        RemoveLeaderboardCommand command = new RemoveLeaderboardCommand(interactionService, leaderboardService);

        await command.RemoveLeaderboardAsync(context, ChannelId, confirm: true);

        await interactionService
            .Received(1)
            .SendResponseAsync(
                Arg.Any<CommandContext>(),
                "An error occurred while processing your command. Please try again later.",
                true
            );
    }

    [Fact]
    public async Task RemoveLeaderboard_reports_service_exception()
    {
        CommandContext context = DiscordCommandTestDoubles.CreateContext();
        var interactionService = DiscordCommandTestDoubles.CreateInteractionService();
        ILeaderboardService leaderboardService = Substitute.For<ILeaderboardService>();
        leaderboardService
            .RemoveLeaderboardAsync(ChannelId)
            .Returns(Task.FromException<bool>(new InvalidOperationException("Service unavailable")));
        RemoveLeaderboardCommand command = new RemoveLeaderboardCommand(interactionService, leaderboardService);

        await command.RemoveLeaderboardAsync(context, ChannelId, confirm: true);

        await interactionService
            .Received(1)
            .SendResponseAsync(
                Arg.Any<CommandContext>(),
                "An error occurred while processing your command. Please try again later.",
                true
            );
    }
}
