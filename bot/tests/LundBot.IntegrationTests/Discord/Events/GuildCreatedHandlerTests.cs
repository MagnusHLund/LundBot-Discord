using LundBot.IntegrationTests.Discord.Events.Support;
using LundBot.Presentation.Discord.Events;
using NSubstitute;
using Serilog.Events;
using Xunit;

namespace LundBot.IntegrationTests.Discord.Events;

public sealed class GuildCreatedHandlerTests : DiscordEventTestBase
{
    [Fact]
    public async Task Guild_created_refreshes_Discord_commands_through_application_service()
    {
        var args = DiscordEventData.GuildCreated(Host.Client);

        await Host.Handler<GuildCreatedHandler>().HandleEventAsync(Host.Client, args);

        await Host.Commands.Received(1).RefreshCommandsAsync();
    }

    [Fact]
    public async Task Command_refresh_exception_is_logged_without_escaping()
    {
        InvalidOperationException exception = new InvalidOperationException("Commands unavailable");
        Host.Commands.RefreshCommandsAsync().Returns(Task.FromException<bool>(exception));

        await Host.Handler<GuildCreatedHandler>()
            .HandleEventAsync(Host.Client, DiscordEventData.GuildCreated(Host.Client));

        Assert.Contains(Host.Logs.Events, entry => entry.Level == LogEventLevel.Error && entry.Exception == exception);
    }
}
