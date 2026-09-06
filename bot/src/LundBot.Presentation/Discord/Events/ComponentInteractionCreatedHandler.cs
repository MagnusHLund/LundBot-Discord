using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using LundBot.Application.Discord.Members;
using LundBot.Application.Features.MemberJoin;
using LundBot.Presentation.Discord.Interactions;

namespace LundBot.Presentation.Discord.Events
{
    public sealed class ComponentInteractionCreatedHandler : IEventHandler<ComponentInteractionCreatedEventArgs>
    {
        private readonly IMemberJoinService _memberJoinService;
        private readonly IDiscordInteractionService _discordInteractionService;
        private readonly IDiscordMemberService _discordMemberService;

        private readonly ILogger _logger = Log.ForContext<ComponentInteractionCreatedHandler>();

        public ComponentInteractionCreatedHandler(
            IMemberJoinService memberJoinService,
            IDiscordInteractionService discordInteractionService,
            IDiscordMemberService discordMemberService
        )
        {
            _memberJoinService = memberJoinService;
            _discordInteractionService = discordInteractionService;
            _discordMemberService = discordMemberService;
        }

        public async Task HandleEventAsync(DiscordClient sender, ComponentInteractionCreatedEventArgs eventArgs)
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

        private async Task RunInteractionAsync(ComponentInteractionCreatedEventArgs eventArgs)
        {
            var (interactionName, targetMember) = await ExtractInteractionDetails(eventArgs);

            switch (interactionName)
            {
                case "memberJoin_hi":
                    if (targetMember is null)
                    {
                        await UnknownInteractionResponse(eventArgs);
                        return;
                    }

                    await AuthorizeUserAndExecute(
                        eventArgs,
                        targetMember!,
                        async () =>
                        {
                            return await _memberJoinService.HandleMemberJoinHiEventAsync(
                                eventArgs.User?.Id ?? 0,
                                targetMember.UserId,
                                eventArgs.Channel?.Id ?? 0,
                                eventArgs.Guild?.Id ?? 0
                            );
                        }
                    );
                    break;
                default:
                    await UnknownInteractionResponse(eventArgs);
                    break;
            }
        }

        private async Task<(string interactionName, DiscordMemberDto? targetMember)> ExtractInteractionDetails(
            ComponentInteractionCreatedEventArgs eventArgs
        )
        {
            string[] interactionParts = eventArgs.Id.Split(':');

            string interactionName = interactionParts[0] ?? eventArgs.Id;
            DiscordMemberDto? targetMember;

            if (interactionParts.Length <= 1)
            {
                return (interactionName, null);
            }

            if (!ulong.TryParse(interactionParts[1], out ulong userId))
            {
                _logger.Error("Error parsing target user ID from interaction: {InteractionId}", eventArgs.Id);

                await _discordInteractionService.SendResponseAsync(
                    eventArgs.Interaction,
                    "Unable to parse target user ID.",
                    true
                );

                return (interactionName, null);
            }

            try
            {
                ulong guildId = eventArgs.Guild?.Id ?? 0;
                targetMember = await _discordMemberService.GetMemberAsync(userId, guildId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error parsing target user ID from interaction: {InteractionId}", eventArgs.Id);

                await _discordInteractionService.SendResponseAsync(
                    eventArgs.Interaction,
                    "Unable to fetch target user. The user might not still be in the server.",
                    true
                );

                return (interactionName, null);
            }

            return (interactionName, targetMember);
        }

        private async Task AuthorizeUserAndExecute(
            InteractionCreatedEventArgs eventArgs,
            DiscordMemberDto targetMember,
            Func<Task<bool>> action
        )
        {
            if (
                await NotifyUserUnauthorizedForOwnAction(
                    eventArgs.Interaction.User.Id,
                    targetMember.UserId,
                    eventArgs.Interaction
                )
            )
            {
                await _discordInteractionService.SendResponseAsync(
                    eventArgs.Interaction,
                    "You are not authorized to perform this action.",
                    true
                );
                return;
            }

            await eventArgs.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredMessageUpdate);

            bool success = await action();

            if (!success)
            {
                await _discordInteractionService.SendResponseAsync(
                    eventArgs.Interaction,
                    "Action failed. Please try again later.",
                    true
                );
            }
        }

        private async Task UnknownInteractionResponse(ComponentInteractionCreatedEventArgs eventArgs)
        {
            _logger.Warning("Unknown interaction received: {InteractionId}", eventArgs.Id);

            string message = $"Unknown interaction! Please contact an administrator.";
            await _discordInteractionService.SendResponseAsync(eventArgs.Interaction, message, showOnlyToUser: true);
        }

        private async Task<bool> NotifyUserUnauthorizedForOwnAction(
            ulong userId,
            ulong targetUserId,
            DiscordInteraction interaction
        )
        {
            if (userId != targetUserId)
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
