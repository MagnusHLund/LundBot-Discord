using DSharpPlus.Entities;

namespace LundBot.Tests.TestHelpers;

internal static class DiscordTestHelper
{
    internal static DiscordChannel TestChannel(ulong id) => DiscordObjectFactory.CreateChannel(id);

    internal static DiscordUser TestUser(ulong id, string username) =>
        DiscordObjectFactory.CreateUser(id, username);

    internal static DiscordMessage TestMessage(ulong id, DiscordChannel channel) =>
        DiscordObjectFactory.CreateMessage(id, channel);
}
