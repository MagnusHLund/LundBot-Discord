using DSharpPlus.Entities;
using LundBot.Interfaces.Services.Discord;
using LundBot.Tests.Fakes.Discord;

namespace LundBot.Tests.Mocks.Discord;

public sealed class MockDiscordChannelService : IDiscordChannelService
{
    public FakeDiscordChannel Channel { get; set; } =
        new FakeDiscordChannel { Id = 123456789012345678 };

    public Task<FakeDiscordChannel> GetChannelAsync(ulong channelId) => Task.FromResult(Channel);
}
