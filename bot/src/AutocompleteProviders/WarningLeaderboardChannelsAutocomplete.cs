using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LundBot.Enums;
using LundBot.Interfaces.Services;

namespace LundBot.AutocompleteProviders
{
    public class WarningLeaderboardChannelsAutocomplete : LeaderboardChannelsAutocomplete
    {
        public WarningLeaderboardChannelsAutocomplete(ILeaderboardService leaderboardService)
            : base(leaderboardService) { }

        public override async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(
            AutocompleteContext context
        )
        {
            var warningLeaderboards = (
                await GetLeaderboardChoicesForGuildAsync(context.Guild.Id, LeaderboardType.Warning)
            );

            return warningLeaderboards.Select(l => new DiscordAutoCompleteChoice(
                l.Title,
                l.DiscordChannelId
            ));
        }
    }
}
