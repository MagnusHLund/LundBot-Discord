using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace LundBot.Tests.TestHelpers;

internal static class DiscordObjectFactory
{
    internal static DiscordChannel CreateChannel(ulong id)
    {
        DiscordChannel channel = CreateUninitialized<DiscordChannel>();
        SetMemberValue(channel, "Id", id);
        return channel;
    }

    internal static DiscordUser CreateUser(ulong id, string username)
    {
        DiscordUser user = CreateUninitialized<DiscordUser>();
        SetMemberValue(user, "Id", id);
        SetMemberValue(user, "Username", username);
        return user;
    }

    internal static DiscordGuild CreateUninitializedGuild(ulong id)
    {
        DiscordGuild guild = CreateUninitialized<DiscordGuild>();
        SetMemberValue(guild, "Id", id);
        SetMemberValue(guild, "_channels", new ConcurrentDictionary<ulong, DiscordChannel>());
        return guild;
    }

    internal static DiscordGuild CreateGuildWithChannel(ulong guildId, DiscordChannel channel)
    {
        var channels = new ConcurrentDictionary<ulong, DiscordChannel>();
        channels[channel.Id] = channel;

        DiscordGuild guild = CreateUninitialized<DiscordGuild>();
        SetMemberValue(guild, "Id", guildId);
        SetMemberValue(guild, "_channels", channels);
        return guild;
    }

    internal static InteractionContext CreateInteractionContext(DiscordUser user, DiscordGuild guild)
    {
        InteractionContext context = CreateUninitialized<InteractionContext>();
        SetMemberValue(context, "User", user);
        SetMemberValue(context, "Guild", guild);
        return context;
    }

    internal static DiscordMessage CreateMessage(ulong id, DiscordChannel channel)
    {
        DiscordMessage message = CreateUninitialized<DiscordMessage>();
        SetMemberValue(message, "Id", id);
        SetMemberValue(message, "Channel", channel);
        return message;
    }

    private static T CreateUninitialized<T>()
        where T : class => (T)RuntimeHelpers.GetUninitializedObject(typeof(T));

    private static void SetMemberValue(object target, string memberName, object value)
    {
        Type type = target.GetType();

        PropertyInfo? property = type.GetProperty(
            memberName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );
        if (property?.SetMethod is not null)
        {
            property.SetValue(target, value);
            return;
        }

        FieldInfo? backingField =
            type.GetField(
                $"<{memberName}>k__BackingField",
                BindingFlags.Instance | BindingFlags.NonPublic
            )
            ?? type.GetField(
                memberName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

        if (backingField is null)
        {
            throw new InvalidOperationException(
                $"Could not set member '{memberName}' on '{type.FullName}'."
            );
        }

        backingField.SetValue(target, value);
    }
}
