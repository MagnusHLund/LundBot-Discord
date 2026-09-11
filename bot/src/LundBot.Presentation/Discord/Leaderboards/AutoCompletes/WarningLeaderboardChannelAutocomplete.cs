using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Domain.Leaderboards;

namespace LundBot.Presentation.Discord.Leaderboards.AutoCompletes
{
    public sealed class WarningLeaderboardChannelAutocomplete : LeaderboardChannelAutocomplete
    {
        public WarningLeaderboardChannelAutocomplete(ILeaderboardService leaderboardService)
            : base(leaderboardService) { }

        public override async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(
            AutoCompleteContext context
        )
        {
            if (context.Guild is null)
            {
                return Enumerable.Empty<DiscordAutoCompleteChoice>();
            }

            var warningLeaderboards = (
                await GetLeaderboardChoicesForGuildAsync(context.Guild.Id, LeaderboardTypeEnum.Warning)
            );

            return warningLeaderboards.Select(l => new DiscordAutoCompleteChoice(l.Title, l.DiscordChannelId));
        }
    }
}
