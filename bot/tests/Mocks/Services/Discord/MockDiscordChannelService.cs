using DSharpPlus.Entities;
using LundBot.Interfaces.Services.Discord;
using LundBot.Tests.TestHelpers;

namespace LundBot.Tests.Mocks.Discord;

public sealed class MockDiscordChannelService : IDiscordChannelService
{
    public DiscordChannel Channel { get; set; } = DiscordTestHelper.TestChannel(123456789012345678);

    public Task<DiscordChannel> GetChannelAsync(ulong channelId) => Task.FromResult(Channel);
}
