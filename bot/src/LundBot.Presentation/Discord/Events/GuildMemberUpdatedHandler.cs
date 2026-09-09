using DSharpPlus;
using DSharpPlus.EventArgs;
using LundBot.Application.Features.Moderation;
using LundBot.Presentation.Config;
using Microsoft.Extensions.Options;

namespace LundBot.Presentation.Discord.Events
{
    public sealed class GuildMemberUpdatedHandler : IEventHandler<GuildMemberUpdatedEventArgs>
    {
        private readonly DiscordCommandConfig _discordConfig;
        private readonly IModerationActionService _moderationActionsService;
        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger _logger = Log.ForContext<GuildMemberUpdatedHandler>();

        public GuildMemberUpdatedHandler(
            IOptions<DiscordCommandConfig> discordConfig,
            IModerationActionService moderationActionsService,
            IServiceProvider serviceProvider
        )
        {
            _moderationActionsService = moderationActionsService;
            _serviceProvider = serviceProvider;
            _discordConfig = discordConfig.Value;
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
