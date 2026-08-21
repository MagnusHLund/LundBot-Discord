using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LundBot.Interfaces.Services.Discord;
using Serilog;

namespace LundBot.Services.Discord
{
    public sealed class DiscordInteractionService : IDiscordInteractionService
    {
        private readonly Serilog.ILogger _logger = Log.ForContext<DiscordInteractionService>();

        public async ValueTask<bool> IsCommandSentFromServer(InteractionContext context)
        {
            if (context.Guild is null)
            {
                await SendResponseAsync(context, "This command can only be used inside a server.");
                return false;
            }
            return true;
        }

        public async Task SendResponseAsync(
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
