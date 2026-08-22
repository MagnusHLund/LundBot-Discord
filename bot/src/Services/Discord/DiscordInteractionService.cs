using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using LundBot.Interfaces.Services;
using LundBot.Interfaces.Services.Discord;
using Serilog;

namespace LundBot.Services.Discord
{
    public sealed class DiscordInteractionService : IDiscordInteractionService
    {
        private readonly IDiscordMessageService _discordMessageService;
        private readonly IWelcomeMessageService _welcomeMessageService;
        private readonly Serilog.ILogger _logger = Log.ForContext<DiscordInteractionService>();

        public DiscordInteractionService(
            IWelcomeMessageService welcomeMessageService,
            IDiscordMessageService discordMessageService
        )
        {
            _welcomeMessageService = welcomeMessageService;
            _discordMessageService = discordMessageService;
        }

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

        public async Task SendResponseAsync(
            DiscordInteraction interaction,
            string content,
            bool showOnlyToUser = true
        )
        {
            await interaction.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
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
                InteractionResponseType.ChannelMessageWithSource,
                responseBuilder.AsEphemeral(showOnlyToUser)
            );
        }

        public async Task HandleComponentInteractionAsync(ComponentInteractionCreateEventArgs e)
        {
            string eventId = e.Id;

            switch (eventId)
            {
                case "welcome_hi":
                    // Acknowledge the button press (required)
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                    await HandleWelcomeInteractionAsync(e.User, e.Channel);
                    break;
                default:
                    await SendResponseAsync(e.Interaction, "Unknown interaction.", true);
                    break;
            }
        }

        private async Task HandleWelcomeInteractionAsync(DiscordUser user, DiscordChannel channel)
        {
            var welcomeStickers = await _welcomeMessageService.GetWelcomeStickersAsync();
            short randomIndex = (short)new Random().Next(welcomeStickers.Count);
            var randomSticker = welcomeStickers[randomIndex];

            var message = new DiscordMessageBuilder()
                .WithSticker(randomSticker)
                .WithContent($"{user.Mention} says hi!");

            await _discordMessageService.SendMessageAsync(channel, message);
        }
    }
}
