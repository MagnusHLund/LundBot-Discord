using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using LundBot.Entities;
using LundBot.Enums;
using LundBot.Interfaces.Services;

namespace LundBot.AutocompleteProviders
{
    public abstract class LeaderboardChannelsAutocomplete : IAutoCompleteProvider
    {
        private protected readonly ILeaderboardService _leaderboardService;

        public LeaderboardChannelsAutocomplete(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        public abstract ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(
            AutoCompleteContext context
        );

        private protected async Task<List<LeaderboardsEntity>> GetLeaderboardChoicesForGuildAsync(
            ulong guildId,
            LeaderboardType? type = null
        )
        {
            List<LeaderboardsEntity> leaderboards =
                await _leaderboardService.GetLeaderboardsForGuildAsync(guildId.ToString());

            if (type.HasValue)
            {
                leaderboards = leaderboards.Where(l => l.LeaderboardType == type.Value).ToList();
            }

            return leaderboards;
        }
    }
}
