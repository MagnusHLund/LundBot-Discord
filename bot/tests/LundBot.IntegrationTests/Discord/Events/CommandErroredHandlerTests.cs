using DSharpPlus.Commands;
using DSharpPlus.Commands.EventArgs;
using LundBot.IntegrationTests.Discord.Events.Support;
using LundBot.Presentation.Discord.Events;
using NSubstitute;
using Serilog.Events;
using Xunit;

namespace LundBot.IntegrationTests.Discord.Events;

public sealed class CommandErroredHandlerTests : DiscordEventTestBase
{
    [Fact]
    public async Task Command_error_sends_private_safe_response()
    {
        CommandErroredEventArgs args = new CommandErroredEventArgs
        {
            CommandObject = this,
            Context = DiscordEventData.Context(),
            Exception = new InvalidOperationException("private details"),
        };
        Host.Interactions.SendResponseAsync(Arg.Any<CommandContext>(), Arg.Any<string>(), true).Returns(true);

        await Host.Handler<CommandErroredHandler>().HandleEventAsync(Host.Client, args);

        await Host
            .Interactions.Received(1)
            .SendResponseAsync(Arg.Any<CommandContext>(), "Internal server error. Please try again later.", true);
    }

    [Fact]
    public async Task Missing_context_does_not_attempt_response()
    {
        CommandErroredEventArgs args = new CommandErroredEventArgs
        {
            CommandObject = this,
            Context = null!,
            Exception = new InvalidOperationException("Command failed"),
        };

        await Host.Handler<CommandErroredHandler>().HandleEventAsync(Host.Client, args);

        await Host
            .Interactions.DidNotReceive()
            .SendResponseAsync(Arg.Any<CommandContext>(), Arg.Any<string>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task Response_failure_is_logged()
    {
        CommandErroredEventArgs args = new CommandErroredEventArgs
        {
            CommandObject = this,
            Context = DiscordEventData.Context(),
            Exception = new InvalidOperationException("Command failed"),
        };
        Host.Interactions.SendResponseAsync(Arg.Any<CommandContext>(), Arg.Any<string>(), true).Returns(false);

        await Host.Handler<CommandErroredHandler>().HandleEventAsync(Host.Client, args);

        Assert.Contains(
            Host.Logs.Events,
            entry =>
                entry.Level == LogEventLevel.Warning
                && entry.MessageTemplate.Text == "Failed to send error response to the user."
        );
    }

    [Fact]
    public async Task Response_exception_is_logged_without_escaping()
    {
        InvalidOperationException exception = new InvalidOperationException("Discord unavailable");
        CommandErroredEventArgs args = new CommandErroredEventArgs
        {
            CommandObject = this,
            Context = DiscordEventData.Context(),
            Exception = new InvalidOperationException("Command failed"),
        };
        Host.Interactions.SendResponseAsync(Arg.Any<CommandContext>(), Arg.Any<string>(), true)
            .Returns(Task.FromException<bool>(exception));

        await Host.Handler<CommandErroredHandler>().HandleEventAsync(Host.Client, args);

        Assert.Contains(Host.Logs.Events, entry => entry.Level == LogEventLevel.Error && entry.Exception == exception);
    }
}
