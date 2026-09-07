using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace LundBot.Presentation.Discord.Interactions
{
    public sealed class DiscordInteractionService : IDiscordInteractionService
    {
        public async ValueTask<bool> IsCommandSentFromGuild(CommandContext context)
        {
            try
            {
                if (context.Guild is null)
                {
                    await SendResponseAsync(context, "This command can only be used inside a server.");
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SendResponseAsync(CommandContext context, string message, bool showToUser = true)
        {
            try
            {
                await context.RespondAsync(
                    new DiscordInteractionResponseBuilder().WithContent(message).AsEphemeral(showToUser)
                );
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SendResponseAsync(
            DiscordInteraction interaction,
            string content,
            bool showOnlyToUser = true
        )
        {
            try
            {
                await interaction.CreateResponseAsync(
                    DiscordInteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(content).AsEphemeral(showOnlyToUser)
                );
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SendResponseAsync(
            DiscordInteraction interaction,
            DiscordInteractionResponseBuilder responseBuilder,
            bool showOnlyToUser = true
        )
        {
            try
            {
                await interaction.CreateResponseAsync(
                    DiscordInteractionResponseType.ChannelMessageWithSource,
                    responseBuilder.AsEphemeral(showOnlyToUser)
                );
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SendFollowUpAsync(
            DiscordInteraction interaction,
            string message,
            bool showOnlyToUser = true
        )
        {
            try
            {
                await interaction.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().WithContent(message).AsEphemeral(showOnlyToUser)
                );

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
