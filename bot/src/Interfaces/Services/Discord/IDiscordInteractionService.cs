using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;

namespace LundBot.Interfaces.Services.Discord
{
    public interface IDiscordInteractionService
    {
        ValueTask<bool> IsCommandSentFromServer(InteractionContext context);
        Task SendResponseAsync(
            InteractionContext context,
            string content,
            bool showOnlyToUser = true
        );

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
