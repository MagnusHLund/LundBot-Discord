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
    }
}
