using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace LundBot.Commands
{
    public sealed class RemoveLeaderboardCommand : BaseCommand
    {
        [SlashRequirePermissions(Permissions.Administrator)]
        [SlashCommand("remove-leaderboard", "Removes an existing leaderboard.")]
        public async Task RemoveLeaderboardAsync(
            InteractionContext context,
            [Option("Channel", "The Channel that the leaderboard is in")] DiscordChannel channel,
            [Option("Confirm", "Confirm the removal of the leaderboard")] bool confirm
        )
        {
            if (!await IsCommandSentFromServer(context))
            {
                return;
            }

            if (!confirm)
            {
                await context.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder()
                        .WithContent(
                            "You must confirm the removal of the leaderboard by setting the 'Confirm' option to true."
                        )
                        .AsEphemeral(true)
                );
                return;
            }
        }
    }
}
