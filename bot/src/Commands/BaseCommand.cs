using DSharpPlus.Commands;
using LundBot.Exceptions;
using LundBot.Interfaces.Services.Discord;

namespace LundBot.Commands
{
    public abstract class BaseCommand
    {
        private readonly IDiscordInteractionService _discordInteractionService;

        public BaseCommand(IDiscordInteractionService discordInteractionService)
        {
            _discordInteractionService = discordInteractionService;
        }

        public const string GENERIC_ERROR_MESSAGE =
            "An error occurred while processing your command. Please try again later.";

        private protected async Task<bool> IsCommandSentFromServer(CommandContext context)
        {
            return await _discordInteractionService.IsCommandSentFromServer(context);
        }

        private protected async Task SendResponseAsync(
            CommandContext context,
            string content,
            bool showOnlyToUser = true
        )
        {
            await _discordInteractionService.SendResponseAsync(context, content, showOnlyToUser);
        }

        private protected async Task TaskWithErrorHandlingAsync(
            CommandContext context,
            Func<Task> action,
            string successMessage = "Command executed successfully."
        )
        {
            try
            {
                await action();
                await SendResponseAsync(context, successMessage);
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
