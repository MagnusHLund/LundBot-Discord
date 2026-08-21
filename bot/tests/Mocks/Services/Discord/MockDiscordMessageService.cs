using DSharpPlus.Entities;
using LundBot.Interfaces.Services.Discord;
using LundBot.Tests.TestHelpers;

namespace LundBot.Tests.Mocks.Services.Discord
{
    public class MockDiscordMessageService : IDiscordMessageService
    {
        public List<(DiscordChannel Channel, string Content)> Sent { get; } = new();
        public List<(DiscordMessage Message, string NewContent)> Modified { get; } = new();
        public List<DiscordMessage> Deleted { get; } = new();

        public Func<ulong, DiscordMessage>? GetMessageBehavior { get; set; }

        public Task<DiscordMessage> GetMessageAsync(DiscordChannel channel, ulong messageId)
        {
            if (GetMessageBehavior is null)
                throw new Exception("Mock GetMessageBehavior not set.");

            return Task.FromResult(GetMessageBehavior(messageId));
        }

        public Task<DiscordMessage> SendMessageAsync(DiscordChannel channel, string content)
        {
            DiscordMessage msg = DiscordTestHelper.TestMessage((ulong)(Sent.Count + 1), channel);
            Sent.Add((channel, content));
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
}
