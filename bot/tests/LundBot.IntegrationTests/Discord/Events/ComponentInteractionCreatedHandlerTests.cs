using DSharpPlus.Entities;
using LundBot.Application.Discord.Members;
using LundBot.Application.Discord.Messages;
using LundBot.IntegrationTests.Discord.Events.Support;
using LundBot.Presentation.Discord.Events;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Serilog.Events;
using Xunit;

namespace LundBot.IntegrationTests.Discord.Events;

public sealed class ComponentInteractionCreatedHandlerTests : DiscordEventTestBase
{
    [Fact]
    public async Task Say_hi_replies_to_persisted_welcome_message()
    {
        await DiscordEventSeed.WelcomeAsync(Host);
        var args = DiscordEventData.Component(Host.Client, $"memberJoin_hi:{DiscordEventData.TargetId}");

        await Host.Handler<ComponentInteractionCreatedHandler>().HandleEventAsync(Host.Client, args);

        await Host
            .Messages.Received(1)
            .SendMessageAsync(
                DiscordEventData.ChannelId,
                Arg.Is<DiscordMessageBuilderDto>(message =>
                    message.ReplyToMessageId == DiscordEventData.MessageId
                    && message.Content == "**Actor** says hi to **Target**"
                )
            );
    }

    [Fact]
    public async Task Authorized_interaction_is_acknowledged_before_sending_message()
    {
        await DiscordEventSeed.WelcomeAsync(Host);
        var args = DiscordEventData.Component(Host.Client, $"memberJoin_hi:{DiscordEventData.TargetId}");
        bool acknowledged = false;
        Host.Messages.SendMessageAsync(Arg.Any<ulong>(), Arg.Any<DiscordMessageBuilderDto>())
            .Returns(_ =>
            {
                acknowledged = Host.Transport.Requests.Count == 1;
                return new DiscordMessageDto(DiscordEventData.MessageId + 1, DiscordEventData.ChannelId, 0, "Hi");
            });

        await Host.Handler<ComponentInteractionCreatedHandler>().HandleEventAsync(Host.Client, args);

        Assert.True(acknowledged);
    }

    [Fact]
    public async Task Own_welcome_button_is_rejected_privately()
    {
        var args = DiscordEventData.Component(
            Host.Client,
            $"memberJoin_hi:{DiscordEventData.TargetId}",
            DiscordEventData.TargetId
        );

        await Host.Handler<ComponentInteractionCreatedHandler>().HandleEventAsync(Host.Client, args);

        await Host
            .Interactions.Received(1)
            .SendResponseAsync(args.Interaction, "You are not authorized to use this interaction.", true);
    }

    [Fact]
    public async Task Rejected_own_button_does_not_send_hi()
    {
        await DiscordEventSeed.WelcomeAsync(Host);
        var args = DiscordEventData.Component(
            Host.Client,
            $"memberJoin_hi:{DiscordEventData.TargetId}",
            DiscordEventData.TargetId
        );

        await Host.Handler<ComponentInteractionCreatedHandler>().HandleEventAsync(Host.Client, args);

        await Host.Messages.DidNotReceive().SendMessageAsync(Arg.Any<ulong>(), Arg.Any<DiscordMessageBuilderDto>());
    }

    [Theory]
    [InlineData("unknown")]
    [InlineData("memberJoin_hi")]
    public async Task Unsupported_or_incomplete_interaction_receives_unknown_response(string id)
    {
        var args = DiscordEventData.Component(Host.Client, id);

        await Host.Handler<ComponentInteractionCreatedHandler>().HandleEventAsync(Host.Client, args);

        await Host
            .Interactions.Received(1)
            .SendResponseAsync(args.Interaction, "Unknown interaction! Please contact an administrator.", true);
    }

    [Fact]
    public async Task Invalid_target_id_receives_parse_error()
    {
        var args = DiscordEventData.Component(Host.Client, "memberJoin_hi:not-an-id");

        await Host.Handler<ComponentInteractionCreatedHandler>().HandleEventAsync(Host.Client, args);

        await Host
            .Interactions.Received(1)
            .SendResponseAsync(args.Interaction, "Unable to parse target user ID.", true);
    }

    [Fact]
    public async Task Missing_target_receives_unknown_response()
    {
        Host.Members.GetMemberAsync(DiscordEventData.TargetId, DiscordEventData.GuildId)
            .Returns((DiscordMemberDto?)null);
        var args = DiscordEventData.Component(Host.Client, $"memberJoin_hi:{DiscordEventData.TargetId}");

        await Host.Handler<ComponentInteractionCreatedHandler>().HandleEventAsync(Host.Client, args);

        await Host
            .Interactions.Received(1)
            .SendResponseAsync(args.Interaction, "Unknown interaction! Please contact an administrator.", true);
    }

    [Fact]
    public async Task Target_lookup_exception_receives_safe_error()
    {
        Host.Members.GetMemberAsync(DiscordEventData.TargetId, DiscordEventData.GuildId)
            .Returns(Task.FromException<DiscordMemberDto?>(new InvalidOperationException("private failure")));
        var args = DiscordEventData.Component(Host.Client, $"memberJoin_hi:{DiscordEventData.TargetId}");

        await Host.Handler<ComponentInteractionCreatedHandler>().HandleEventAsync(Host.Client, args);

        await Host
            .Interactions.Received(1)
            .SendResponseAsync(
                args.Interaction,
                "Unable to fetch target user. The user might not still be in the server.",
                true
            );
    }

    [Fact]
    public async Task Missing_welcome_message_receives_failure_followup()
    {
        var args = DiscordEventData.Component(Host.Client, $"memberJoin_hi:{DiscordEventData.TargetId}");

        await Host.Handler<ComponentInteractionCreatedHandler>().HandleEventAsync(Host.Client, args);

        await Host
            .Interactions.Received(1)
            .SendFollowUpAsync(args.Interaction, "Action failed. Please try again later.", true);
    }

    [Fact]
    public async Task Discord_message_failure_receives_failure_followup()
    {
        await DiscordEventSeed.WelcomeAsync(Host);
        Host.Messages.SendMessageAsync(Arg.Any<ulong>(), Arg.Any<DiscordMessageBuilderDto>())
            .Returns((DiscordMessageDto?)null);
        var args = DiscordEventData.Component(Host.Client, $"memberJoin_hi:{DiscordEventData.TargetId}");

        await Host.Handler<ComponentInteractionCreatedHandler>().HandleEventAsync(Host.Client, args);

        await Host
            .Interactions.Received(1)
            .SendFollowUpAsync(args.Interaction, "Action failed. Please try again later.", true);
    }

    [Fact]
    public async Task Repository_failure_is_logged_without_escaping()
    {
        await Host.Db.Database.ExecuteSqlRawAsync("DROP TABLE MemberJoinMessages");
        var args = DiscordEventData.Component(Host.Client, $"memberJoin_hi:{DiscordEventData.TargetId}");

        await Host.Handler<ComponentInteractionCreatedHandler>().HandleEventAsync(Host.Client, args);

        Assert.Contains(
            Host.Logs.Events,
            entry =>
                entry.Level == LogEventLevel.Error
                && entry.Exception is LundBot.Application.Common.Exceptions.RepositoryException
        );
    }

    [Fact]
    public async Task Missing_sender_receives_failure_followup()
    {
        await DiscordEventSeed.WelcomeAsync(Host);
        Host.Members.GetMemberAsync(DiscordEventData.UserId, DiscordEventData.GuildId).Returns((DiscordMemberDto?)null);
        var args = DiscordEventData.Component(Host.Client, $"memberJoin_hi:{DiscordEventData.TargetId}");

        await Host.Handler<ComponentInteractionCreatedHandler>().HandleEventAsync(Host.Client, args);

        await Host
            .Interactions.Received(1)
            .SendFollowUpAsync(args.Interaction, "Action failed. Please try again later.", true);
    }

    [Fact]
    public async Task Acknowledgment_failure_is_logged_without_escaping()
    {
        Host.Transport.RejectRequests = true;
        var args = DiscordEventData.Component(Host.Client, $"memberJoin_hi:{DiscordEventData.TargetId}");

        await Host.Handler<ComponentInteractionCreatedHandler>().HandleEventAsync(Host.Client, args);

        Assert.Contains(
            Host.Logs.Events,
            entry =>
                entry.Level == LogEventLevel.Error
                && entry.MessageTemplate.Text.StartsWith("Error handling component", StringComparison.Ordinal)
        );
    }

    [Fact]
    public async Task Acknowledgment_failure_does_not_execute_action()
    {
        await DiscordEventSeed.WelcomeAsync(Host);
        Host.Transport.RejectRequests = true;
        var args = DiscordEventData.Component(Host.Client, $"memberJoin_hi:{DiscordEventData.TargetId}");

        await Host.Handler<ComponentInteractionCreatedHandler>().HandleEventAsync(Host.Client, args);

        await Host.Messages.DidNotReceive().SendMessageAsync(Arg.Any<ulong>(), Arg.Any<DiscordMessageBuilderDto>());
    }
}
