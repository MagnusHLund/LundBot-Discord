using DSharpPlus.Commands.EventArgs;
using LundBot.IntegrationTests.Discord.Events.Support;
using LundBot.Presentation.Discord.Events;
using Serilog.Events;
using Xunit;

namespace LundBot.IntegrationTests.Discord.Events;

public sealed class CommandExecutedHandlerTests : DiscordEventTestBase
{
    [Fact]
    public async Task Executed_command_logs_command_name()
    {
        CommandExecutedEventArgs args = new CommandExecutedEventArgs
        {
            CommandObject = this,
            Context = DiscordEventData.Context(),
        };

        await Host.Handler<CommandExecutedHandler>().HandleEventAsync(Host.Client, args);

        Assert.Contains(
            Host.Logs.Events,
            entry =>
                entry.Level == LogEventLevel.Information
                && entry.Properties.TryGetValue("Cmd", out var value)
                && value is ScalarValue { Value: "test-command" }
        );
    }

    [Fact]
    public async Task Direct_message_command_logs_zero_guild_id()
    {
        CommandExecutedEventArgs args = new CommandExecutedEventArgs
        {
            CommandObject = this,
            Context = DiscordEventData.Context(),
        };

        await Host.Handler<CommandExecutedHandler>().HandleEventAsync(Host.Client, args);

        Assert.Contains(
            Host.Logs.Events,
            entry => entry.Properties.TryGetValue("Guild", out var value) && value is ScalarValue { Value: 0UL }
        );
    }
}
