using DSharpPlus.EventArgs;
using LundBot.IntegrationTests.Discord.Events.Support;
using LundBot.Presentation.Discord.Events;
using NSubstitute;
using Serilog.Events;
using Xunit;

namespace LundBot.IntegrationTests.Discord.Events;

public sealed class SessionCreatedHandlerTests : DiscordEventTestBase
{
    [Fact]
    public async Task Session_sets_bot_status()
    {
        var args = DiscordEventData.Create<SessionCreatedEventArgs>();

        await Host.Handler<SessionCreatedHandler>().HandleEventAsync(Host.Client, args);

        await Host.Bot.Received(1).UpdateBotStatusAsync("Stuck in a movie theater");
    }

    [Fact]
    public async Task Session_preloads_each_guild()
    {
        DiscordEventData.AddGuild(Host.Client, DiscordEventData.GuildId + 1);

        await Host.Handler<SessionCreatedHandler>()
            .HandleEventAsync(Host.Client, DiscordEventData.Create<SessionCreatedEventArgs>());

        ulong[] ids = Host
            .Members.ReceivedCalls()
            .Where(call => call.GetMethodInfo().Name == nameof(Host.Members.PreloadMembersAsync))
            .Select(call => (ulong)call.GetArguments()[0]!)
            .Order()
            .ToArray();
        Assert.Equal(new[] { DiscordEventData.GuildId, DiscordEventData.GuildId + 1 }, ids);
    }

    [Fact]
    public async Task Failed_preload_is_logged()
    {
        Host.Members.PreloadMembersAsync(DiscordEventData.GuildId).Returns(false);

        await Host.Handler<SessionCreatedHandler>()
            .HandleEventAsync(Host.Client, DiscordEventData.Create<SessionCreatedEventArgs>());

        Assert.Contains(
            Host.Logs.Events,
            entry =>
                entry.Level == LogEventLevel.Error
                && entry.MessageTemplate.Text.StartsWith("Failed to preload", StringComparison.Ordinal)
        );
    }

    [Fact]
    public async Task Failed_preload_does_not_skip_other_guilds()
    {
        DiscordEventData.AddGuild(Host.Client, DiscordEventData.GuildId + 1);
        Host.Members.PreloadMembersAsync(Arg.Any<ulong>()).Returns(false);

        await Host.Handler<SessionCreatedHandler>()
            .HandleEventAsync(Host.Client, DiscordEventData.Create<SessionCreatedEventArgs>());

        await Host.Members.Received(1).PreloadMembersAsync(DiscordEventData.GuildId + 1);
    }

    [Fact]
    public async Task Status_exception_propagates_to_dispatcher()
    {
        InvalidOperationException exception = new InvalidOperationException("Status unavailable");
        Host.Bot.UpdateBotStatusAsync(Arg.Any<string>()).Returns(Task.FromException<bool>(exception));

        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Host.Handler<SessionCreatedHandler>()
                .HandleEventAsync(Host.Client, DiscordEventData.Create<SessionCreatedEventArgs>())
        );

        Assert.Same(exception, thrown);
    }
}
