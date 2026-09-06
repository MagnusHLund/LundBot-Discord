using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using LundBot.Application.Features.Leaderboards;
using LundBot.Domain.Leaderboards;

namespace LundBot.Presentation.Discord.Leaderboards.AutoCompletes
{
    public sealed class UpvoteLeaderboardChannelAutocomplete : LeaderboardChannelAutocomplete
    {
        public UpvoteLeaderboardChannelAutocomplete(ILeaderboardService leaderboardService)
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
                await GetLeaderboardChoicesForGuildAsync(context.Guild.Id, LeaderboardTypeEnum.Upvote)
            );

            return upvoteLeaderboards.Select(l => new DiscordAutoCompleteChoice(l.Title, l.DiscordChannelId));
        }
    }
}
