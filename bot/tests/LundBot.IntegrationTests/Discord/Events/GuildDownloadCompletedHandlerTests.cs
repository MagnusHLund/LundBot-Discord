using DSharpPlus.EventArgs;
using LundBot.Application.Common.Caching;
using LundBot.Application.Discord.Invites;
using LundBot.IntegrationTests.Discord.Events.Support;
using LundBot.Presentation.Discord.Events;
using NSubstitute;
using Xunit;

namespace LundBot.IntegrationTests.Discord.Events;

public sealed class GuildDownloadCompletedHandlerTests : DiscordEventTestBase
{
    [Fact]
    public async Task Download_caches_returned_invites()
    {
        List<DiscordInviteDto> invites = new List<DiscordInviteDto> { new("invite", 3, null) };
        Host.Guilds.GetGuildInvitesAsync(DiscordEventData.GuildId).Returns(invites);

        await Host.Handler<GuildDownloadCompletedHandler>()
            .HandleEventAsync(
                Host.Client,
                DiscordEventData.Create<GuildDownloadCompletedEventArgs>(Host.Client.Guilds)
            );

        Assert.Equal(invites, Host.Cache.Get<List<DiscordInviteDto>>(CacheKeys.GuildInvites(DiscordEventData.GuildId)));
    }

    [Fact]
    public async Task Download_caches_each_guild_separately()
    {
        DiscordEventData.AddGuild(Host.Client, DiscordEventData.GuildId + 1);
        List<DiscordInviteDto> invites = new List<DiscordInviteDto> { new("second", 2, null) };
        Host.Guilds.GetGuildInvitesAsync(DiscordEventData.GuildId + 1).Returns(invites);

        await Host.Handler<GuildDownloadCompletedHandler>()
            .HandleEventAsync(
                Host.Client,
                DiscordEventData.Create<GuildDownloadCompletedEventArgs>(Host.Client.Guilds)
            );

        Assert.Equal(
            invites,
            Host.Cache.Get<List<DiscordInviteDto>>(CacheKeys.GuildInvites(DiscordEventData.GuildId + 1))
        );
    }

    [Fact]
    public async Task Failed_download_preserves_previous_cache()
    {
        List<DiscordInviteDto> oldInvites = new List<DiscordInviteDto> { new("previous", 1, null) };
        Host.Cache.Set(CacheKeys.GuildInvites(DiscordEventData.GuildId), oldInvites);
        Host.Guilds.GetGuildInvitesAsync(DiscordEventData.GuildId)
            .Returns(
                Task.FromException<IReadOnlyList<DiscordInviteDto>>(
                    new InvalidOperationException("Invites unavailable")
                )
            );

        await Record.ExceptionAsync(() =>
            Host.Handler<GuildDownloadCompletedHandler>()
                .HandleEventAsync(
                    Host.Client,
                    DiscordEventData.Create<GuildDownloadCompletedEventArgs>(Host.Client.Guilds)
                )
        );

        Assert.Equal(
            oldInvites,
            Host.Cache.Get<List<DiscordInviteDto>>(CacheKeys.GuildInvites(DiscordEventData.GuildId))
        );
    }

    [Fact]
    public async Task Download_exception_propagates_to_dispatcher()
    {
        InvalidOperationException exception = new InvalidOperationException("Invites unavailable");
        Host.Guilds.GetGuildInvitesAsync(DiscordEventData.GuildId)
            .Returns(Task.FromException<IReadOnlyList<DiscordInviteDto>>(exception));

        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Host.Handler<GuildDownloadCompletedHandler>()
                .HandleEventAsync(
                    Host.Client,
                    DiscordEventData.Create<GuildDownloadCompletedEventArgs>(Host.Client.Guilds)
                )
        );

        Assert.Same(exception, thrown);
    }
}
