using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using LundBot.AutocompleteProviders;
using LundBot.Interfaces.Services;
using LundBot.Interfaces.Services.Discord;

namespace LundBot.Commands
{
    public sealed class UpvoteUserOnLeaderboardCommand : BaseCommand
    {
        private readonly ILeaderboardService _leaderboardService;

        public UpvoteUserOnLeaderboardCommand(
            ILeaderboardService leaderboardService,
            IDiscordInteractionService discordInteractionService
        )
            : base(discordInteractionService)
        {
            _leaderboardService = leaderboardService;
        }

        [Command("upvote")]
        [Description("Upvotes a user on the specified leaderboard.")]
        public async Task UpvoteUserAsync(
            CommandContext context,
            [SlashAutoCompleteProvider(typeof(UpvoteLeaderboardChannelsAutocomplete))]
            [Parameter("channel")]
            [Description("The Channel that has the leaderboard")]
                string channelId,
            [Parameter("user")] [Description("The user to upvote.")] DiscordUser userTarget
        )
        {
            if (!await IsCommandSentFromServer(context))
            {
                return;
            }

            if (
                !ulong.TryParse(channelId, out var parsedId)
                || await context.Guild.GetChannelAsync(parsedId) is not DiscordChannel channel
            )
            {
                await SendResponseAsync(context, "The specified channel does not exist.");
                return;
            }

            DiscordUser userUpvoting = context.User;

            if (userUpvoting.Id == userTarget.Id)
            {
                await SendResponseAsync(context, "You cannot upvote yourself on the leaderboard.");
                return;
            }

            await TaskWithErrorHandlingAsync(
                context,
                () =>
                    _leaderboardService.UpvoteUserOnLeaderboardAsync(
                        channel,
                        userUpvoting,
                        userTarget
                    ),
                $"You have successfully upvoted {userTarget.Username} on the leaderboard in {channel.Mention}."
            );
        }
    }
}
