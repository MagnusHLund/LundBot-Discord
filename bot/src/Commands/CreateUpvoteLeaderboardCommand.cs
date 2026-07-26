using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using LundBot.Utils;

namespace LundBot.Commands
{
    public sealed class CreateUpvoteLeaderboardCommand : BaseCommand
    {
        [SlashRequirePermissions(Permissions.Administrator)]
        [SlashCommand("create-upvote-leaderboard", "Creates a new upvote leaderboard.")]
        public async Task CreateUpvoteLeaderboardAsync(
            InteractionContext context,
            [Option("Channel", "The Channel that the leaderboard will use")]
            [ChannelTypes(ChannelType.Text)]
                DiscordChannel Channel,
            [Option(
                "title",
                "The title of the leaderboard. eg 'Top Upvoted Users'. Max 64 characters."
            )]
            [MinimumLength(1)]
            [MaximumLength(64)]
                string title,
            [Option(
                "message",
                "Message to prepend above the leaderboard data. Max 256 characters."
            )]
            [MaximumLength(256)]
                string? message
        )
        {
            if (!await IsCommandSentFromServer(context))
            {
                return;
            }

            title = title.Trim();
            if (!ValidationUtils.IsValidLengthString(title, 64, 1))
            {
                await context.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder()
                        .WithContent("The title must be between 1 and 64 characters long.")
                        .AsEphemeral(true)
                );
                return;
            }

            message = message != null ? message.Trim() : "";
            if (!ValidationUtils.IsValidLengthString(message, 256, 0))
            {
                await context.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder()
                        .WithContent("The message must be between 0 and 256 characters long.")
                        .AsEphemeral(true)
                );
                return;
            }

            // TODO: Implement
        }
    }
}
