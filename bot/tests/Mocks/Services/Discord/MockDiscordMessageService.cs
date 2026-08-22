using DSharpPlus.Entities;
using LundBot.Interfaces.Services.Discord;
using LundBot.Tests.TestHelpers;

namespace LundBot.Tests.Mocks.Services.Discord;

internal sealed class MockDiscordMessageService : IDiscordMessageService
{
    internal List<(DiscordChannel Channel, DiscordMessageBuilder Builder)> Sent { get; } = new();
    internal List<(DiscordChannel Channel, string Content, List<DiscordComponent> Components)> SentWithComponents
    {
        get;
    } = new();
    internal List<(DiscordMessage Message, string NewContent)> Modified { get; } = new();
    internal List<DiscordMessage> Deleted { get; } = new();

    internal Func<ulong, DiscordMessage>? GetMessageBehavior { get; set; }

    public Task<DiscordMessage> GetMessageAsync(DiscordChannel channel, ulong messageId)
    {
        if (GetMessageBehavior is null)
            throw new Exception("Mock GetMessageBehavior not set.");

        return Task.FromResult(GetMessageBehavior(messageId));
    }

    public Task<DiscordMessage> SendMessageAsync(DiscordChannel channel, DiscordMessageBuilder builder)
    {
        DiscordMessage msg = DiscordTestHelper.TestMessage((ulong)(Sent.Count + 1), channel);
        Sent.Add((channel, builder));
        return Task.FromResult(msg);
    }

    public Task<DiscordMessage> SendMessageWithComponentsAsync(
        DiscordChannel channel,
        string content,
        List<DiscordComponent> components
    )
    {
        DiscordMessage msg = DiscordTestHelper.TestMessage((ulong)(SentWithComponents.Count + 100), channel);
        SentWithComponents.Add((channel, content, components));
        return Task.FromResult(msg);
    }

    public Task<DiscordMessage> ModifyMessageAsync(DiscordMessage message, string content)
    {
        Modified.Add((message, content));
        return Task.FromResult(message);
    }

    public Task DeleteMessageAsync(DiscordMessage message)
    {
        Deleted.Add(message);
        return Task.CompletedTask;
    }
}
