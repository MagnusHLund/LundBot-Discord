using DSharpPlus.Commands;
using DSharpPlus.Entities;
using LundBot.Interfaces.Services.Discord;
using Serilog;

namespace LundBot.Services.Discord
{
    public sealed class DiscordInteractionService : IDiscordInteractionService
    {
        private readonly Serilog.ILogger _logger = Log.ForContext<DiscordInteractionService>();

        public async ValueTask<bool> IsCommandSentFromServer(CommandContext context)
        {
            if (context.Guild is null)
            {
                await SendResponseAsync(context, "This command can only be used inside a server.");
                return false;
            }
            return true;
        }

        public async Task SendResponseAsync(
            CommandContext context,
            string content,
            bool showOnlyToUser = true
        )
        {
            await context.RespondAsync(
                new DiscordInteractionResponseBuilder()
                    .WithContent(content)
                    .AsEphemeral(showOnlyToUser)
            );
        }

        public async Task SendResponseAsync(
            DiscordInteraction interaction,
            string content,
            bool showOnlyToUser = true
        )
        {
            await interaction.CreateResponseAsync(
                DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent(content)
                    .AsEphemeral(showOnlyToUser)
            );
        }

        public async Task SendResponseAsync(
            DiscordInteraction interaction,
            DiscordInteractionResponseBuilder responseBuilder,
            bool showOnlyToUser = true
        )
        {
            await interaction.CreateResponseAsync(
                DiscordInteractionResponseType.ChannelMessageWithSource,
                responseBuilder.AsEphemeral(showOnlyToUser)
            );
        }
    }
}
