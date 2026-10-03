using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace LundBot.Presentation.Discord.Interactions
{
    public interface IDiscordInteractionService
    {
        ValueTask<bool> IsCommandSentFromGuild(CommandContext context);

        Task<bool> SendResponseAsync(CommandContext context, string message, bool showOnlyToUser = true);

        Task<bool> SendResponseAsync(DiscordInteraction interaction, string message, bool showOnlyToUser = true);

        Task<bool> SendResponseAsync(
            DiscordInteraction interaction,
            DiscordInteractionResponseBuilder responseBuilder,
            bool showOnlyToUser = true
        );

        Task<bool> SendFollowUpAsync(
            DiscordInteraction interaction,
            string message,
            bool showOnlyToUser = true
        );
    }
}
