using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using LundBot.Enums;
using LundBot.Interfaces.Services;

namespace LundBot.AutocompleteProviders
{
    public class UpvoteLeaderboardChannelsAutocomplete : LeaderboardChannelsAutocomplete
    {
        public UpvoteLeaderboardChannelsAutocomplete(ILeaderboardService leaderboardService)
            : base(leaderboardService) { }

        public override async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(
            AutoCompleteContext context
        )
        {
            if (context.Guild is null)
            {
                return Enumerable.Empty<DiscordAutoCompleteChoice>();
            }

            var upvoteLeaderboards = (
                await GetLeaderboardChoicesForGuildAsync(context.Guild.Id, LeaderboardType.Upvote)
            );

            return upvoteLeaderboards.Select(l => new DiscordAutoCompleteChoice(
                l.Title,
                l.DiscordChannelId
            ));
        }
    }
}
