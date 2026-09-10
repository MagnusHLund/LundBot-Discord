using DSharpPlus;
using DSharpPlus.EventArgs;
using LundBot.Application.Discord.Roles;
using LundBot.Application.Features.Moderation;
using LundBot.Presentation.Config;
using Microsoft.Extensions.Options;

namespace LundBot.Presentation.Discord.Events
{
    public sealed class GuildMemberUpdatedHandler : IEventHandler<GuildMemberUpdatedEventArgs>
    {
        private readonly DiscordCommandConfig _discordCommandConfig;
        private readonly DiscordKickConfig _discordKickConfig;
        private readonly IModerationActionService _moderationActionsService;
        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger _logger = Log.ForContext<GuildMemberUpdatedHandler>();

        public GuildMemberUpdatedHandler(
            IOptions<DiscordCommandConfig> discordCommandConfig,
            IOptions<DiscordKickConfig> discordKickConfig,
            IModerationActionService moderationActionsService,
            IServiceProvider serviceProvider
        )
        {
            _moderationActionsService = moderationActionsService;
            _serviceProvider = serviceProvider;
            _discordCommandConfig = discordCommandConfig.Value;
            _discordKickConfig = discordKickConfig.Value;
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

            var roleToKick = eventArgs.Member.Roles.FirstOrDefault(r => r.Id == _discordKickConfig.RoleIdToAutoKick);

            if (roleToKick is null)
            {
                return;
            }

            DiscordRoleDto roleToKickDto = new DiscordRoleDto(roleToKick.Id, roleToKick.Name);

            string kickReason =
                $"You have been automatically kicked due to picking the \"{roleToKick?.Name ?? "Unknown"}\" role. This community is PC only. Feel free to join back, if you own IW on PC or plan to purchase it on PC";

            await _moderationActionsService.KickUserDueToRoleAssignmentAsync(
                eventArgs.Guild.Id,
                eventArgs.Member.Id,
                roleToKickDto,
                kickReason
            );
        }
    }
}
