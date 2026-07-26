using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace LundBot.Commands
{
    public sealed class UpvoteUserOnLeaderboardCommand : BaseCommand
    {
        [SlashRequirePermissions(Permissions.All)]
        [SlashCommand("upvote", "Upvotes a user on the specified leaderboard.")]
        public async Task UpvoteUserAsync(
            InteractionContext context,
            [Option("channel", "The Channel that the leaderboard is in")] DiscordChannel channel,
            [Option("user", "The user to upvote")] DiscordUser user
        )
        {
            if (!await IsCommandSentFromServer(context))
            {
                return;
            }

            // TODO: Implement
        }
    }
}
