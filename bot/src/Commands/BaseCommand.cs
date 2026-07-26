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
                await SendResponseAsync(context, "This command can only be used inside a server.");
                return false;
            }
            return true;
        }

        private protected static async Task SendResponseAsync(
            InteractionContext context,
            string content,
            bool showOnlyToUser = true
        )
        {
            await context.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent(content)
                    .AsEphemeral(showOnlyToUser)
            );
        }
    }
}
