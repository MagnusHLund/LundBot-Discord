using DSharpPlus.Entities;
using LundBot.Commands;
using LundBot.Interfaces.Services;
using LundBot.Tests.Mocks.Services.Discord;
using LundBot.Tests.TestHelpers;
using Moq;

namespace LundBot.Tests.Unit.Commands;

public sealed class WarnOnLeaderboardCommandTests
{
    [Fact]
    internal async Task RegisterWarningAsync_WhenCommandNotFromServer_DoesNothing()
    {
        // Arrange
        var interactionService = new MockDiscordInteractionService
        {
            IsCommandSentFromServerResult = false,
        };
        var leaderboardService = new Mock<ILeaderboardService>();
        var command = new WarnOnLeaderboardCommand(leaderboardService.Object, interactionService);
        var target = DiscordTestHelper.TestUser(100, "Target");

        // Act
        await command.RegisterWarningAsync(null!, "12345", target);

        // Assert
        leaderboardService.Verify(
            s =>
                s.RegisterWarningOnLeaderboardAsync(
                    It.IsAny<DiscordChannel>(),
                    It.IsAny<DiscordUser>()
                ),
            Times.Never
        );
        Assert.Empty(interactionService.Responses);
    }

    [Fact]
    internal async Task RegisterWarningAsync_WhenChannelIdIsInvalid_SendsChannelNotFoundError()
    {
        // Arrange
        var interactionService = new MockDiscordInteractionService();
        var leaderboardService = new Mock<ILeaderboardService>();
        var command = new WarnOnLeaderboardCommand(leaderboardService.Object, interactionService);
        var target = DiscordTestHelper.TestUser(100, "Target");

        // Act
        await command.RegisterWarningAsync(null!, "not-a-number", target);

        // Assert
        Assert.Single(interactionService.Responses);
        Assert.Equal(
            "The specified channel does not exist.",
            interactionService.Responses[0].Content
        );
        leaderboardService.Verify(
            s =>
                s.RegisterWarningOnLeaderboardAsync(
                    It.IsAny<DiscordChannel>(),
                    It.IsAny<DiscordUser>()
                ),
            Times.Never
        );
    }

    [Fact]
    internal async Task RegisterWarningAsync_WhenChannelNotFoundInGuild_SendsChannelNotFoundError()
    {
        // Arrange
        var interactionService = new MockDiscordInteractionService();
        var leaderboardService = new Mock<ILeaderboardService>();
        var command = new WarnOnLeaderboardCommand(leaderboardService.Object, interactionService);
        var user = DiscordTestHelper.TestUser(1, "Admin");
        var emptyGuild = DiscordObjectFactory.CreateUninitializedGuild(99);
        var context = DiscordObjectFactory.CreateCommandContext(user, emptyGuild);
        var target = DiscordTestHelper.TestUser(200, "Target");

        // Act
        await command.RegisterWarningAsync(context, "99999", target);

        // Assert
        Assert.Single(interactionService.Responses);
        Assert.Equal(
            "The specified channel does not exist.",
            interactionService.Responses[0].Content
        );
        leaderboardService.Verify(
            s =>
                s.RegisterWarningOnLeaderboardAsync(
                    It.IsAny<DiscordChannel>(),
                    It.IsAny<DiscordUser>()
                ),
            Times.Never
        );
    }

    [Fact]
    internal async Task RegisterWarningAsync_WhenValidInput_InvokesServiceAndSendsSuccess()
    {
        // Arrange
        var interactionService = new MockDiscordInteractionService();
        var leaderboardService = new Mock<ILeaderboardService>();
        var command = new WarnOnLeaderboardCommand(leaderboardService.Object, interactionService);
        var channel = DiscordTestHelper.TestChannel(12345);
        var guild = DiscordObjectFactory.CreateGuildWithChannel(10, channel);
        var user = DiscordTestHelper.TestUser(1, "Admin");
        var userTarget = DiscordTestHelper.TestUser(300, "Target");
        var context = DiscordObjectFactory.CreateCommandContext(user, guild);

        // Act
        await command.RegisterWarningAsync(context, "12345", userTarget);

        // Assert
        leaderboardService.Verify(
            s => s.RegisterWarningOnLeaderboardAsync(channel, userTarget),
            Times.Once
        );
        Assert.Single(interactionService.Responses);
        Assert.Contains("Registered a warning", interactionService.Responses[0].Content);
    }
}
