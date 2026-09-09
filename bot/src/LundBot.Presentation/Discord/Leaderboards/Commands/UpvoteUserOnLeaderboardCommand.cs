using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using LundBot.Application.Discord.Users;
using LundBot.Application.Features.Leaderboards;
using LundBot.Presentation.Discord.Bot;
using LundBot.Presentation.Discord.Interactions;
using LundBot.Presentation.Discord.Leaderboards.AutoCompletes;

namespace LundBot.Presentation.Discord.Leaderboards.Commands
{
    public sealed class UpvoteUserOnLeaderboardCommand : AbstractBaseCommand
    {
        private readonly ILeaderboardService _leaderboardService;

        public UpvoteUserOnLeaderboardCommand(
            IDiscordInteractionService discordInteractionService,
            ILeaderboardService leaderboardService
        )
            : base(discordInteractionService)
        {
            _leaderboardService = leaderboardService;
        }

        [Command("upvote")]
        [Description("Upvote a user on a specified leaderboard.")]
        public async Task UpvoteUserAsync(
            CommandContext context,
            [SlashAutoCompleteProvider(typeof(UpvoteLeaderboardChannelAutocomplete))]
            [Parameter("channel")]
            [Description("The Channel that has the leaderboard")]
                ulong channelId,
            [Parameter("user")] [Description("The user to upvote.")] DiscordUser user
        )
        {
            if (!await IsCommandSentFromGuild(context))
            {
                return;
            }

            DiscordUserDto userUpvoting = new DiscordUserDto(context.User.Id, context.User.Username);

            if (userUpvoting.UserId == user.Id)
            {
                await SendResponseAsync(context, "You cannot upvote yourself on the leaderboard.");
                return;
            }

            DiscordUserDto targetUser = new DiscordUserDto(user.Id, user.Username);

            await TaskWithErrorHandlingAsync(
                context,
                () => _leaderboardService.UpvoteUserOnLeaderboardAsync(channelId, userUpvoting, targetUser),
                $"You have successfully upvoted {targetUser.Username} on the leaderboard in <#{channelId}>."
            );
        }
    }
}
