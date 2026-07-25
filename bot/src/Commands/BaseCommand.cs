using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace LundBot.Commands
{
    public class BaseCommand : ApplicationCommandModule
    {
        private protected static async Task<bool> IsCommandSentFromServer(
            InteractionContext context
        )
        {
            if (context.Guild is null)
            {
                await context.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder()
                        .WithContent("This command can only be used inside a server.")
                        .AsEphemeral(true)
                );
                return false;
            }
            return true;
        }
    }
}
