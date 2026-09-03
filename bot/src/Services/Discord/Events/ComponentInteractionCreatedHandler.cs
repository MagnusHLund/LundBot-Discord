using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using LundBot.Interfaces.Services;
using LundBot.Interfaces.Services.Discord;

namespace LundBot.Services.Discord.Events
{
    public sealed class ComponentInteractionCreatedHandler
        : IEventHandler<ComponentInteractionCreatedEventArgs>
    {
        private readonly IWelcomeMessageService _welcomeMessageService;
        private readonly IDiscordInteractionService _discordInteractionService;
        private readonly IDiscordMemberService _discordMemberService;

        private readonly Serilog.ILogger _logger =
            Serilog.Log.ForContext<ComponentInteractionCreatedHandler>();

        public ComponentInteractionCreatedHandler(
            IWelcomeMessageService welcomeMessageService,
            IDiscordInteractionService discordInteractionService,
            IDiscordMemberService discordMemberService
        )
        {
            _welcomeMessageService = welcomeMessageService;
            _discordInteractionService = discordInteractionService;
            _discordMemberService = discordMemberService;
        }

        public async Task HandleEventAsync(
            DiscordClient sender,
            ComponentInteractionCreatedEventArgs eventArgs
        )
        {
            _logger.Information(
                "Component interaction created: {CustomId} by {User} in Guild={Guild}",
                eventArgs.Id,
                eventArgs.User?.Username,
                eventArgs.Guild?.Id ?? 0
            );

            try
            {
                await RunInteractionAsync(eventArgs);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error handling component interaction: {CustomId} by {User} in Guild={Guild}",
                    eventArgs.Id,
                    eventArgs.User?.Username,
                    eventArgs.Guild?.Id ?? 0
                );
            }
        }

        private async Task RunInteractionAsync(ComponentInteractionCreatedEventArgs e)
        {
            string[] interactionParts = e.Id.Split(':');

            string interactionName = interactionParts[0] ?? e.Id;
            DiscordMember? discordMember = null;

            if (interactionParts.Length > 1)
            {
                ulong userId = ulong.Parse(interactionParts[1]);
                discordMember = await e.Guild.GetMemberAsync(userId);
            }

            switch (interactionName)
            {
                case "welcome_hi":
                    if (
                        await NotifyUserUnauthorizedForOwnAction(
                            e.User,
                            discordMember!.Id,
                            e.Interaction
                        )
                    )
                    {
                        return;
                    }

                    // Acknowledge the button press (required)
                    await e.Interaction.CreateResponseAsync(
                        DiscordInteractionResponseType.DeferredMessageUpdate
                    );

                    List<Task<DiscordMember>> tasks = new List<Task<DiscordMember>>
                    {
                        _discordMemberService.GetMemberAsync(e.Guild, e.User.Id),
                        _discordMemberService.GetMemberAsync(e.Guild, discordMember!.Id),
                    };

                    DiscordMember[] members = await Task.WhenAll(tasks);
                    DiscordMember senderMember = members[0];
                    DiscordMember targetMember = members[1];

                    await _welcomeMessageService.HandleWelcomeInteractionAsync(
                        senderMember,
                        targetMember,
                        e.Channel
                    );
                    break;
                default:
                    await _discordInteractionService.SendResponseAsync(
                        e.Interaction,
                        "Unknown interaction.",
                        true
                    );
                    break;
            }
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

            await _discordInteractionService.SendResponseAsync(
                interaction,
                "You are not authorized to use this interaction."
            );
            return true;
        }
    }
}
