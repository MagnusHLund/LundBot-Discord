using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using LundBot.Application.Features.Leaderboards;
using LundBot.Domain.Leaderboards;

namespace LundBot.Presentation.Discord.Leaderboards.AutoCompletes
{
    public class LeaderboardChannelAutocomplete : IAutoCompleteProvider
    {
        private protected readonly ILeaderboardService _leaderboardService;

        public LeaderboardChannelAutocomplete(ILeaderboardService leaderboardService)
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

            var leaderboards = await _leaderboardService.GetLeaderboardsForGuildAsync(guildId.Value);

            return leaderboards.Select(l => new DiscordAutoCompleteChoice(l.Title, l.DiscordChannelId));
        }

        private protected async Task<IEnumerable<Leaderboard>> GetLeaderboardChoicesForGuildAsync(
            ulong guildId,
            LeaderboardTypeEnum? type = null
        )
        {
            IEnumerable<Leaderboard> leaderboards = await _leaderboardService.GetLeaderboardsForGuildAsync(guildId);

            if (type.HasValue)
            {
                leaderboards = leaderboards.Where(l => l.LeaderboardType == type.Value);
            }

            return leaderboards;
        }
    }
}
