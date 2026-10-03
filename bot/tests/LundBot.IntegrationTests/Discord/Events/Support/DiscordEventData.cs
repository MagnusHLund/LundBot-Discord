using System.Collections.Concurrent;
using System.Reflection;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using NSubstitute;

namespace LundBot.IntegrationTests.Discord.Events.Support;

internal static class DiscordEventData
{
    public const ulong GuildId = 1531716596022644876;
    public const ulong ChannelId = 1531717575338102914;
    public const ulong UserId = 1531717575338102915;
    public const ulong TargetId = 1531717575338102916;
    public const ulong RoleId = 1531717575338102917;
    public const ulong MessageId = 1531717575338102918;

    // Gateway event arguments and cached SDK entities have internal constructors/setters.
    public static T Create<T>(params object[] arguments)
        where T : class =>
        Activator.CreateInstance(
            typeof(T),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            args: arguments,
            culture: null
        ) as T
        ?? throw new InvalidOperationException($"Cannot construct SDK fixture {typeof(T).Name}.");

    public static void Set(object instance, string property, object? value)
    {
        var member =
            instance
                .GetType()
                .GetProperty(property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException($"SDK property {property} not found.");
        if (member.SetMethod is null)
        {
            throw new InvalidOperationException($"SDK property {property} has no setter.");
        }
        member.SetValue(instance, value);
    }

    private static T Field<T>(object instance, string name)
        where T : class =>
        instance.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(instance) as T
        ?? throw new InvalidOperationException($"SDK field {name} not found.");

    public static DiscordGuild AddGuild(DiscordClient client, ulong id = GuildId)
    {
        var guild = Create<DiscordGuild>();
        Set(guild, "Id", id);
        Set(guild, "Name", "Test guild");
        Set(guild, "Discord", client);
        var channelsField =
            typeof(DiscordGuild).GetField("channels", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException("SDK guild channels field not found.");
        channelsField.SetValue(guild, new ConcurrentDictionary<ulong, DiscordChannel>());
        Field<ConcurrentDictionary<ulong, DiscordGuild>>(client, "guilds")[id] = guild;
        return guild;
    }

    public static DiscordMember Member(DiscordClient client, bool withRole = false)
    {
        var user = User(UserId);
        Set(user, "Discriminator", "1234");
        Set(user, "Discord", client);
        var users =
            typeof(BaseDiscordClient)
                .GetProperty("UserCache", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(client) as ConcurrentDictionary<ulong, DiscordUser>
            ?? throw new InvalidOperationException("SDK user cache not found.");
        users[UserId] = user;
        var member = Create<DiscordMember>(user);
        Set(member, "Discord", client);
        var guildField =
            typeof(DiscordMember).GetField("guild_id", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException("SDK member guild field not found.");
        guildField.SetValue(member, GuildId);
        if (withRole)
        {
            var role = Create<DiscordRole>();
            Set(role, "Id", RoleId);
            Set(role, "Name", "Onboarding");
            Field<ConcurrentDictionary<ulong, DiscordRole>>(client.Guilds[GuildId], "roles")[RoleId] = role;
            Field<List<ulong>>(member, "role_ids").Add(RoleId);
        }
        return member;
    }

    private static DiscordUser User(ulong id)
    {
        var user = Create<DiscordUser>();
        Set(user, "Id", id);
        Set(user, "Username", "Test user");
        return user;
    }

    public static GuildMemberAddedEventArgs MemberAdded(DiscordClient client)
    {
        var args = Create<GuildMemberAddedEventArgs>();
        Set(args, "Guild", client.Guilds[GuildId]);
        Set(args, "Member", Member(client));
        return args;
    }

    public static GuildMemberUpdatedEventArgs MemberUpdated(DiscordClient client, bool withRole = true)
    {
        var args = Create<GuildMemberUpdatedEventArgs>();
        Set(args, "Guild", client.Guilds[GuildId]);
        Set(args, "MemberAfter", Member(client, withRole));
        return args;
    }

    public static GuildCreatedEventArgs GuildCreated(DiscordClient client)
    {
        var args = Create<GuildCreatedEventArgs>();
        Set(args, "Guild", client.Guilds[GuildId]);
        return args;
    }

    public static ComponentInteractionCreatedEventArgs Component(
        DiscordClient client,
        string id,
        ulong actorId = UserId
    )
    {
        DiscordChannel channel = Create<DiscordChannel>();
        Set(channel, "Id", ChannelId);
        Set(channel, "Discord", client);
        Set(channel, "GuildId", GuildId);
        Field<ConcurrentDictionary<ulong, DiscordChannel>>(client.Guilds[GuildId], "channels")[ChannelId] = channel;
        DiscordInteractionData data = new DiscordInteractionData();
        Set(data, "CustomId", id);
        DiscordInteraction interaction = new DiscordInteraction();
        Set(interaction, "Id", MessageId + 1);
        Set(interaction, "User", User(actorId));
        Set(interaction, "Data", data);
        Set(interaction, "GuildId", GuildId);
        Set(interaction, "ChannelId", ChannelId);
        Set(interaction, "ContextChannel", channel);
        Set(interaction, "Token", "offline-test-token");
        Set(interaction, "Discord", client);
        var args = Create<ComponentInteractionCreatedEventArgs>();
        Set(args, "Interaction", interaction);
        return args;
    }

    public static CommandContext Context()
    {
        var context = Substitute.For<CommandContext>();
        Set(
            context,
            "Command",
            new Command([], [])
            {
                Name = "test-command",
                Method = typeof(DiscordEventData).GetMethod(nameof(Context))!,
                Id = default,
                Attributes = [],
            }
        );
        Set(context, "User", User(UserId));
        var channel = Create<DiscordDmChannel>();
        Set(channel, "Id", ChannelId);
        Set(context, "Channel", channel);
        return context;
    }
}
