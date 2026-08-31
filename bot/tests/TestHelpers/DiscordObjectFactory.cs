using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using DSharpPlus.Commands;
using DSharpPlus.Entities;

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

    internal static DiscordMember CreateMember(ulong id, bool? isPending = null)
    {
        DiscordMember member = CreateUninitialized<DiscordMember>();
        SetMemberValue(member, "Id", id);
        if (isPending.HasValue)
            SetMemberValue(member, "IsPending", isPending.Value);
        return member;
    }

    internal static DiscordRole CreateRole(ulong id)
    {
        DiscordRole role = CreateUninitialized<DiscordRole>();
        SetMemberValue(role, "Id", id);
        return role;
    }

    internal static DiscordGuild CreateUninitializedGuild(ulong id)
    {
        DiscordGuild guild = CreateUninitialized<DiscordGuild>();
        SetMemberValue(guild, "Id", id);
        SetMemberValue(guild, "channels", new ConcurrentDictionary<ulong, DiscordChannel>());
        return guild;
    }

    internal static DiscordGuild CreateGuildWithChannel(ulong guildId, DiscordChannel channel)
    {
        var channels = new ConcurrentDictionary<ulong, DiscordChannel>();
        channels[channel.Id] = channel;

        DiscordGuild guild = CreateUninitialized<DiscordGuild>();
        SetMemberValue(guild, "Id", guildId);
        SetMemberValue(guild, "channels", channels);
        return guild;
    }

    internal static DiscordGuild CreateGuildWithSystemChannel(
        ulong guildId,
        DiscordChannel systemChannel
    )
    {
        var channels = new ConcurrentDictionary<ulong, DiscordChannel>();
        channels[systemChannel.Id] = systemChannel;

        DiscordGuild guild = CreateUninitialized<DiscordGuild>();
        SetMemberValue(guild, "Id", guildId);
        SetMemberValue(guild, "channels", channels);
        // DSharpPlus stores this as a property named _systemChannelId (Nullable<ulong>)
        SetMemberValue(guild, "_systemChannelId", (ulong?)systemChannel.Id);
        return guild;
    }

    internal static CommandContext CreateCommandContext(DiscordUser user, DiscordGuild guild)
    {
        CommandContext context = CreateUninitialized<CommandContext>();
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

        // Walk inheritance chain so backing fields on base classes are also found.
        Type? current = type;
        FieldInfo? backingField = null;
        while (current is not null && backingField is null)
        {
            var test = typeof(DiscordGuild).GetFields(
                BindingFlags.Instance
                    | BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.DeclaredOnly
            );

            backingField =
                current.GetField(
                    $"<{memberName}>k__BackingField",
                    BindingFlags.Instance | BindingFlags.NonPublic
                )
                ?? current.GetField(
                    memberName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                );
            current = current.BaseType;
        }

        if (backingField is null)
        {
            throw new InvalidOperationException(
                $"Could not set member '{memberName}' on '{type.FullName}'."
            );
        }

        backingField.SetValue(target, value);
    }
}
