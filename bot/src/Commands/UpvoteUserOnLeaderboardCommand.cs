using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using LundBot.AutocompleteProviders;
using LundBot.Interfaces.Services;

namespace LundBot.Commands
{
    public sealed class UpvoteUserOnLeaderboardCommand : BaseCommand
    {
        private readonly ILeaderboardService _leaderboardService;

        public UpvoteUserOnLeaderboardCommand(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [SlashRequirePermissions(Permissions.All)]
        [SlashCommand("upvote", "Upvotes a user on the specified leaderboard.")]
        public async Task UpvoteUserAsync(
            InteractionContext context,
            [Autocomplete(typeof(UpvoteLeaderboardChannelsAutocomplete))]
            [Option("channel", "The Channel that the leaderboard is in")]
                DiscordChannel channel,
            [Option("user", "The user to upvote")] DiscordUser userTarget
        )
        {
            if (!await IsCommandSentFromServer(context))
            {
                return;
            }

            DiscordUser userUpvoting = context.User;

            if (userUpvoting.Id == userTarget.Id)
            {
                await SendResponseAsync(context, "You cannot upvote yourself on the leaderboard.");
                return;
            }

            await TaskWithErrorHandlingAsync(
                context,
                () =>
                    _leaderboardService.UpvoteUserOnLeaderboard(channel, userUpvoting, userTarget),
                $"You have successfully upvoted {userTarget.Username} on the leaderboard in {channel.Mention}."
            );
        }
    }
}
