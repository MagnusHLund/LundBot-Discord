using System.Reflection;
using System.Runtime.CompilerServices;
using DSharpPlus.Entities;

namespace LundBot.Tests.TestHelpers;

internal static class DiscordObjectFactory
{
    public static DiscordChannel CreateChannel(ulong id)
    {
        DiscordChannel channel = CreateUninitialized<DiscordChannel>();
        SetMemberValue(channel, "Id", id);
        return channel;
    }

    public static DiscordUser CreateUser(ulong id, string username)
    {
        DiscordUser user = CreateUninitialized<DiscordUser>();
        SetMemberValue(user, "Id", id);
        SetMemberValue(user, "Username", username);
        return user;
    }

    public static DiscordGuild CreateUninitializedGuild(ulong id)
    {
        DiscordGuild guild = CreateUninitialized<DiscordGuild>();
        SetMemberValue(guild, "Id", id);
        return guild;
    }

    public static DiscordMessage CreateMessage(ulong id, DiscordChannel channel)
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
            ?? type.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (backingField is null)
        {
            throw new InvalidOperationException(
                $"Could not set member '{memberName}' on '{type.FullName}'."
            );
        }

        backingField.SetValue(target, value);
    }
}
