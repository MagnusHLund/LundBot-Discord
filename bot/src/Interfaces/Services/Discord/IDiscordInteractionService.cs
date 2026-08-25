using DSharpPlus.Commands;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace LundBot.Interfaces.Services.Discord
{
    public interface IDiscordInteractionService
    {
        ValueTask<bool> IsCommandSentFromServer(CommandContext context);
        Task SendResponseAsync(CommandContext context, string content, bool showOnlyToUser = true);

        Task SendResponseAsync(
            DiscordInteraction interaction,
            string content,
            bool showOnlyToUser = true
        );

        Task SendResponseAsync(
            DiscordInteraction interaction,
            DiscordInteractionResponseBuilder responseBuilder,
            bool showOnlyToUser = true
        );
        Task HandleComponentInteractionAsync(ComponentInteractionCreatedEventArgs e);
    }
}
