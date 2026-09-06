using DSharpPlus.Commands;
using LundBot.Application.Common.Exceptions;
using LundBot.Presentation.Discord.Interactions;

namespace LundBot.Presentation.Discord.Bot
{
    public abstract class AbstractBaseCommand
    {
        private readonly IDiscordInteractionService _discordInteractionService;

        public AbstractBaseCommand(IDiscordInteractionService discordInteractionService)
        {
            _discordInteractionService = discordInteractionService;
        }

        private protected async Task<bool> IsCommandSentFromGuild(CommandContext context)
        {
            return await _discordInteractionService.IsCommandSentFromGuild(context);
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
                await SendResponseAsync(
                    context,
                    "An error occurred while processing your command. Please try again later."
                );
            }
        }
    }
}
