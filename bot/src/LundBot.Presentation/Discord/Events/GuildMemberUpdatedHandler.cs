using DSharpPlus;
using DSharpPlus.EventArgs;
using LundBot.Application.Features.Moderation;

namespace LundBot.Presentation.Discord.Events
{
    public sealed class GuildMemberUpdatedHandler : IEventHandler<GuildMemberUpdatedEventArgs>
    {
        private readonly IModerationActionService _moderationActionsService;

        private readonly ILogger _logger = Log.ForContext<GuildMemberUpdatedHandler>();

        public GuildMemberUpdatedHandler(IModerationActionService moderationActionsService)
        {
            _moderationActionsService = moderationActionsService;
        }

        public async Task HandleEventAsync(DiscordClient sender, GuildMemberUpdatedEventArgs eventArgs)
        {
            _logger.Information(
                "Member updated: {UserName} ({UserId}) in guild {GuildName} ({GuildId})",
                eventArgs.Member.Username,
                eventArgs.Member.Id,
                eventArgs.Guild.Name,
                eventArgs.Guild.Id
            );

            await _moderationActionsService.KickUserDueToRoleAssignmentAsync(eventArgs.Guild.Id, eventArgs.Member.Id);
        }
    }
}
