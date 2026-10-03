using DSharpPlus.Commands;
using LundBot.Presentation.Discord.Bot;
using LundBot.Presentation.Discord.InfiniteWarfare.Maps;
using LundBot.Presentation.Discord.Leaderboards.Commands;
using Xunit;

namespace LundBot.IntegrationTests.Discord.Commands;

public sealed class DiscordCommandRegistrationTests
{
    [Fact]
    public void Registered_command_types_have_expected_slash_command_names()
    {
        Type[] commandTypes =
        [
            typeof(CreateLeaderboardCommand),
            typeof(RemoveLeaderboardCommand),
            typeof(UpvoteUserOnLeaderboardCommand),
            typeof(WarnOnLeaderboardCommand),
            typeof(RandomMapCommand),
            typeof(PingCommand),
        ];

        string[] commandNames = commandTypes
            .SelectMany(type => type.GetMethods())
            .SelectMany(method => method.GetCustomAttributes(typeof(CommandAttribute), inherit: true))
            .Cast<CommandAttribute>()
            .Select(attribute => attribute.Name)
            .Order()
            .ToArray();

        Assert.Equal(
            ["create-leaderboard", "ping", "random-map", "remove-leaderboard", "upvote", "warn"],
            commandNames
        );
    }
}
