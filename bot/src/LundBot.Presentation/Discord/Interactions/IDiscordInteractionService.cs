using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace LundBot.Presentation.Discord.Interactions
{
    public interface IDiscordInteractionService
    {
        ValueTask<bool> IsCommandSentFromGuild(CommandContext context);

        Task<bool> SendResponseAsync(CommandContext context, string message, bool showToUser = true);

        Task<bool> SendResponseAsync(DiscordInteraction interaction, string content, bool showOnlyToUser = true);

        Task<bool> SendResponseAsync(
            DiscordInteraction interaction,
            DiscordInteractionResponseBuilder responseBuilder,
            bool showOnlyToUser = true
        );
    }
}
