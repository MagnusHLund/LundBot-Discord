using LundBot.Application.Discord.Roles;
using LundBot.IntegrationTests.Discord.Events.Support;
using LundBot.Presentation.Discord.Events;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace LundBot.IntegrationTests.Discord.Events;

public sealed class GuildMemberUpdatedHandlerTests : DiscordEventTestBase
{
    [Fact]
    public async Task Configured_role_kicks_member_with_stored_reason()
    {
        await DiscordEventSeed.KickRoleAsync(Host);
        Host.Moderation.KickMemberAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>()).Returns(true);

        await Host.Handler<GuildMemberUpdatedHandler>()
            .HandleEventAsync(Host.Client, DiscordEventData.MemberUpdated(Host.Client));

        await Host
            .Moderation.Received(1)
            .KickMemberAsync(DiscordEventData.UserId, DiscordEventData.GuildId, "Onboarding rule");
    }

    [Fact]
    public async Task Unconfigured_role_does_not_kick_member()
    {
        var args = DiscordEventData.MemberUpdated(Host.Client);

        await Host.Handler<GuildMemberUpdatedHandler>().HandleEventAsync(Host.Client, args);

        await Host.Moderation.DidNotReceive().KickMemberAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Member_without_matching_roles_is_not_kicked()
    {
        await DiscordEventSeed.KickRoleAsync(Host);
        Host.Roles.GetAllRolesForMemberAsync(Arg.Any<ulong>(), Arg.Any<ulong>()).Returns(new List<DiscordRoleDto>());

        await Host.Handler<GuildMemberUpdatedHandler>()
            .HandleEventAsync(Host.Client, DiscordEventData.MemberUpdated(Host.Client, false));

        await Host.Moderation.DidNotReceive().KickMemberAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Kick_exception_propagates_to_dispatcher()
    {
        await DiscordEventSeed.KickRoleAsync(Host);
        InvalidOperationException exception = new InvalidOperationException("Kick unavailable");
        Host.Moderation.KickMemberAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>())
            .Returns(Task.FromException<bool>(exception));

        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Host.Handler<GuildMemberUpdatedHandler>()
                .HandleEventAsync(Host.Client, DiscordEventData.MemberUpdated(Host.Client))
        );

        Assert.Same(exception, thrown);
    }

    [Fact]
    public async Task Failed_kick_preserves_role_configuration()
    {
        await DiscordEventSeed.KickRoleAsync(Host);
        Host.Moderation.KickMemberAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>()).Returns(false);

        await Host.Handler<GuildMemberUpdatedHandler>()
            .HandleEventAsync(Host.Client, DiscordEventData.MemberUpdated(Host.Client));

        Assert.Equal(1, await Host.ReadAsync(db => db.AutoKickRoles.CountAsync()));
    }
}
