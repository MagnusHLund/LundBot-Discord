using DSharpPlus.Entities;
using LundBot.Interfaces.Services.Discord;
using LundBot.Tests.TestHelpers;

namespace LundBot.Tests.Mocks.Services.Discord;

internal sealed class MockDiscordChannelService : IDiscordChannelService
{
    internal DiscordChannel Channel { get; set; } =
        DiscordTestHelper.TestChannel(123456789012345678);

    public Task<DiscordChannel> GetChannelAsync(ulong channelId) => Task.FromResult(Channel);

    public Task<DiscordChannel> GetSystemChannelAsync(DiscordGuild guild) =>
        Task.FromResult(Channel);
}
