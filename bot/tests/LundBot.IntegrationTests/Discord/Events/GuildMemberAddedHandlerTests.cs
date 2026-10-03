using LundBot.Application.Discord.Channels;
using LundBot.Application.Discord.Interactions;
using LundBot.Application.Discord.Invites;
using LundBot.Application.Discord.Messages;
using LundBot.Application.Discord.Users;
using LundBot.IntegrationTests.Discord.Events.Support;
using LundBot.Presentation.Discord.Events;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace LundBot.IntegrationTests.Discord.Events;

public sealed class GuildMemberAddedHandlerTests : DiscordEventTestBase
{
    [Fact]
    public async Task Member_added_persists_welcome_message()
    {
        var args = DiscordEventData.MemberAdded(Host.Client);

        await Host.Handler<GuildMemberAddedHandler>().HandleEventAsync(Host.Client, args);

        var message = await Host.ReadAsync(db => db.MemberJoinMessages.SingleAsync());
        Assert.Equal(DiscordEventData.UserId, message.DiscordUserId);
        Assert.Equal(DiscordEventData.MessageId, message.DiscordMessageId);
    }

    [Fact]
    public async Task Welcome_contains_button_targeting_joined_member()
    {
        var args = DiscordEventData.MemberAdded(Host.Client);

        await Host.Handler<GuildMemberAddedHandler>().HandleEventAsync(Host.Client, args);

        await Host
            .Messages.Received(1)
            .SendMessageWithComponentsAsync(
                DiscordEventData.ChannelId,
                Arg.Any<string>(),
                Arg.Is<IReadOnlyCollection<AbstractDiscordMessageComponentDto>>(components =>
                    components.OfType<DiscordButtonDto>().Single().CustomId
                    == $"memberJoin_hi:{DiscordEventData.UserId}"
                )
            );
    }

    [Fact]
    public async Task Used_invite_persists_inviter_score()
    {
        await DiscordEventSeed.InviteLeaderboardAsync(Host);
        DiscordEventSeed.InviteUsed(Host, new DiscordUserDto(DiscordEventData.TargetId, "Inviter", null));

        await Host.Handler<GuildMemberAddedHandler>()
            .HandleEventAsync(Host.Client, DiscordEventData.MemberAdded(Host.Client));

        var score = await Host.ReadAsync(db => db.LeaderboardScores.SingleAsync());
        Assert.Equal(DiscordEventData.TargetId, score.DiscordUserId);
        Assert.Equal(1, score.Score);
    }

    [Fact]
    public async Task Invite_without_inviter_does_not_add_score()
    {
        await DiscordEventSeed.InviteLeaderboardAsync(Host);
        DiscordEventSeed.InviteUsed(Host, null);

        await Host.Handler<GuildMemberAddedHandler>()
            .HandleEventAsync(Host.Client, DiscordEventData.MemberAdded(Host.Client));

        Assert.Equal(0, await Host.ReadAsync(db => db.LeaderboardScores.CountAsync()));
    }

    [Fact]
    public async Task Unchanged_invite_count_does_not_add_score()
    {
        await DiscordEventSeed.InviteLeaderboardAsync(Host);
        DiscordUserDto inviter = new DiscordUserDto(DiscordEventData.TargetId, "Inviter", null);
        List<DiscordInviteDto> invites = new List<DiscordInviteDto> { new("invite", 1, inviter) };
        Host.Cache.Set(LundBot.Application.Common.Caching.CacheKeys.GuildInvites(DiscordEventData.GuildId), invites);
        Host.Guilds.GetGuildInvitesAsync(DiscordEventData.GuildId).Returns(invites);

        await Host.Handler<GuildMemberAddedHandler>()
            .HandleEventAsync(Host.Client, DiscordEventData.MemberAdded(Host.Client));

        Assert.Equal(0, await Host.ReadAsync(db => db.LeaderboardScores.CountAsync()));
    }

    [Fact]
    public async Task Used_invite_persists_score_source_for_joined_member()
    {
        await DiscordEventSeed.InviteLeaderboardAsync(Host);
        DiscordEventSeed.InviteUsed(Host, new DiscordUserDto(DiscordEventData.TargetId, "Inviter", null));

        await Host.Handler<GuildMemberAddedHandler>()
            .HandleEventAsync(Host.Client, DiscordEventData.MemberAdded(Host.Client));

        var source = await Host.ReadAsync(db => db.LeaderboardScoreSources.SingleAsync());
        Assert.Equal(DiscordEventData.UserId, source.DiscordUserIdActor);
        Assert.Equal(DiscordEventData.TargetId, source.DiscordUserIdTarget);
    }

    [Fact]
    public async Task Missing_invite_leaderboard_does_not_add_score()
    {
        DiscordEventSeed.InviteUsed(Host, new DiscordUserDto(DiscordEventData.TargetId, "Inviter", null));

        await Host.Handler<GuildMemberAddedHandler>()
            .HandleEventAsync(Host.Client, DiscordEventData.MemberAdded(Host.Client));

        Assert.Equal(0, await Host.ReadAsync(db => db.LeaderboardScores.CountAsync()));
    }

    [Fact]
    public async Task Missing_system_channel_does_not_persist_welcome()
    {
        Host.Channels.GetSystemChannelAsync(DiscordEventData.GuildId).Returns((DiscordChannelDto?)null);

        await Host.Handler<GuildMemberAddedHandler>()
            .HandleEventAsync(Host.Client, DiscordEventData.MemberAdded(Host.Client));

        Assert.Equal(0, await Host.ReadAsync(db => db.MemberJoinMessages.CountAsync()));
    }

    [Fact]
    public async Task Discord_send_failure_does_not_persist_welcome()
    {
        Host.Messages.SendMessageWithComponentsAsync(
                Arg.Any<ulong>(),
                Arg.Any<string>(),
                Arg.Any<IReadOnlyCollection<AbstractDiscordMessageComponentDto>>()
            )
            .Returns((DiscordMessageDto?)null);

        await Host.Handler<GuildMemberAddedHandler>()
            .HandleEventAsync(Host.Client, DiscordEventData.MemberAdded(Host.Client));

        Assert.Equal(0, await Host.ReadAsync(db => db.MemberJoinMessages.CountAsync()));
    }

    [Fact]
    public async Task Invite_fetch_exception_propagates_to_dispatcher()
    {
        InvalidOperationException exception = new InvalidOperationException("Discord unavailable");
        Host.Guilds.GetGuildInvitesAsync(DiscordEventData.GuildId)
            .Returns(Task.FromException<IReadOnlyList<DiscordInviteDto>>(exception));

        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Host.Handler<GuildMemberAddedHandler>()
                .HandleEventAsync(Host.Client, DiscordEventData.MemberAdded(Host.Client))
        );

        Assert.Same(exception, thrown);
    }
}
