using DSharpPlus.Entities;

namespace LundBot.Tests.TestHelpers;

public static class DiscordTestHelper
{
    public static DiscordChannel TestChannel(ulong id) => DiscordObjectFactory.CreateChannel(id);

    public static DiscordUser TestUser(ulong id, string username) =>
        DiscordObjectFactory.CreateUser(id, username);

    public static DiscordMessage TestMessage(ulong id, DiscordChannel channel) =>
        DiscordObjectFactory.CreateMessage(id, channel);
}
