using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using LundBot.Interfaces.Services;
using LundBot.Interfaces.Services.Discord;
using Serilog;

namespace LundBot.Services.Discord
{
    public sealed class DiscordInteractionService : IDiscordInteractionService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IDiscordMessageService _discordMessageService;
        private readonly Serilog.ILogger _logger = Log.ForContext<DiscordInteractionService>();

        public DiscordInteractionService(
            IServiceScopeFactory scopeFactory,
            IDiscordMessageService discordMessageService
        )
        {
            _scopeFactory = scopeFactory;
            _discordMessageService = discordMessageService;
        }

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

        public async Task HandleComponentInteractionAsync(ComponentInteractionCreatedEventArgs e)
        {
            string[] interactionParts = e.Id.Split(':');

            string interactionName = interactionParts[0];
            ulong userId = ulong.Parse(interactionParts[1]);

            DiscordMember discordMember = await e.Guild.GetMemberAsync(userId);

            switch (interactionName)
            {
                case "welcome_hi":
                    if (await NotifyUserUnauthorizedForOwnAction(e.User, userId, e.Interaction))
                    {
                        return;
                    }

                    // Acknowledge the button press (required)
                    await e.Interaction.CreateResponseAsync(
                        DiscordInteractionResponseType.DeferredMessageUpdate
                    );
                    await HandleWelcomeInteractionAsync(discordMember, e.Channel);
                    break;
                default:
                    await SendResponseAsync(e.Interaction, "Unknown interaction.", true);
                    break;
            }
        }

        private async Task HandleWelcomeInteractionAsync(DiscordMember user, DiscordChannel channel)
        {
            using var scope = _scopeFactory.CreateScope();
            var welcomeMessageService =
                scope.ServiceProvider.GetRequiredService<IWelcomeMessageService>();

            var welcomeStickers = await welcomeMessageService.GetWelcomeStickersAsync();
            short randomIndex = (short)new Random().Next(welcomeStickers.Count);
            var randomSticker = welcomeStickers[randomIndex];

            var message = new DiscordMessageBuilder().WithContent($"{user.Mention} says hi!");

            if (randomSticker is not null)
            {
                message.WithStickers([randomSticker]);
            }

            await _discordMessageService.SendMessageAsync(channel, message);
        }

        private async Task<bool> NotifyUserUnauthorizedForOwnAction(
            DiscordUser user,
            ulong targetUserId,
            DiscordInteraction interaction
        )
        {
            if (user.Id != targetUserId)
            {
                return false;
            }

            await SendResponseAsync(interaction, "You are not authorized to use this interaction.");
            return true;
        }
    }
}
