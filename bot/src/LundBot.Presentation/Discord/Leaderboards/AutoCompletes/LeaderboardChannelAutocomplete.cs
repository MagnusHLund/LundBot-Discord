using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Domain.Leaderboards;

namespace LundBot.Presentation.Discord.Leaderboards.AutoCompletes
{
    public class LeaderboardChannelAutocomplete : IAutoCompleteProvider
    {
        private protected readonly ILeaderboardService _leaderboardService;

        private readonly ILogger _logger = Log.ForContext<LeaderboardChannelAutocomplete>();

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

            IEnumerable<Leaderboard> leaderboards = await GetLeaderboardChoicesForGuildAsync(guildId.Value);

            return leaderboards.Select(l => new DiscordAutoCompleteChoice(l.Title, l.DiscordChannelId));
        }

        private protected async Task<IEnumerable<Leaderboard>> GetLeaderboardChoicesForGuildAsync(
            ulong guildId,
            LeaderboardTypeEnum? type = null
        )
        {
            IEnumerable<Leaderboard> leaderboards;

            try
            {
                leaderboards = await _leaderboardService.GetLeaderboardsForGuildAsync(guildId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error retrieving leaderboards for guild {GuildId} for autocomplete.", guildId);
                return Enumerable.Empty<Leaderboard>();
            }

            if (type.HasValue)
            {
                leaderboards = leaderboards.Where(l => l.LeaderboardType == type.Value);
            }

            return leaderboards;
        }
    }
}
