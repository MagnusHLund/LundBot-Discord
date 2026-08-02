using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LundBot.Entities;
using LundBot.Enums;
using LundBot.Interfaces.Services;

namespace LundBot.AutocompleteProviders
{
    public class UpvoteLeaderboardChannelsAutocomplete : LeaderboardChannelsAutocomplete
    {
        public UpvoteLeaderboardChannelsAutocomplete(ILeaderboardService leaderboardService)
            : base(leaderboardService) { }

        public override async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(
            AutocompleteContext context
        )
        {
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
