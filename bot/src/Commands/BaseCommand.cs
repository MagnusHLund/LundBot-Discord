using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LundBot.Exceptions;

namespace LundBot.Commands
{
    public abstract class BaseCommand : ApplicationCommandModule
    {
        public const string GENERIC_ERROR_MESSAGE =
            "An error occurred while processing your command. Please try again later.";

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

        private protected static async Task TaskWithErrorHandlingAsync(
            InteractionContext context,
            Func<Task> action
        )
        {
            try
            {
                // TODO: Add custom success message

                await action();
                await SendResponseAsync(context, "Command executed successfully.");
            }
            catch (CommandException ex)
            {
                await SendResponseAsync(context, ex.GetMessage());
            }
            catch (Exception)
            {
                await SendResponseAsync(context, GENERIC_ERROR_MESSAGE);
            }
        }
    }
}
