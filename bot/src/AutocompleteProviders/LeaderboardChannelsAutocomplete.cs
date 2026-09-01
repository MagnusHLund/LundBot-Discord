using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using LundBot.Entities;
using LundBot.Enums;
using LundBot.Interfaces.Services;

namespace LundBot.AutocompleteProviders
{
    public class LeaderboardChannelsAutocomplete : IAutoCompleteProvider
    {
        private protected readonly ILeaderboardService _leaderboardService;

        public LeaderboardChannelsAutocomplete(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        public virtual async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(
            AutoCompleteContext context
        )
        {
            ulong? guildId = context.Guild?.Id;

            if (guildId is null)
            {
                return Enumerable.Empty<DiscordAutoCompleteChoice>();
            }

            var leaderboards = await _leaderboardService.GetLeaderboardsForGuildAsync(
                guildId.Value.ToString()
            );

            return leaderboards.Select(l => new DiscordAutoCompleteChoice(
                l.Title,
                l.DiscordChannelId
            ));
        }

        private protected async Task<
            IEnumerable<LeaderboardsEntity>
        > GetLeaderboardChoicesForGuildAsync(ulong guildId, LeaderboardType? type = null)
        {
            IEnumerable<LeaderboardsEntity> leaderboards =
                await _leaderboardService.GetLeaderboardsForGuildAsync(guildId.ToString());

            if (type.HasValue)
            {
                leaderboards = leaderboards.Where(l => l.LeaderboardType == type.Value);
            }

            return leaderboards;
        }
    }
}
