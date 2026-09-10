using LundBot.Application.Discord.Moderation;
using LundBot.Application.Discord.Roles;

namespace LundBot.Application.Features.Moderation
{
    public sealed class ModerationActionService : IModerationActionService
    {
        private readonly IDiscordRoleService _discordRoleService;
        private readonly IDiscordModerationService _discordModerationService;

        public ModerationActionService(
            IDiscordRoleService discordRoleService,
            IDiscordModerationService discordModerationService
        )
        {
            _discordRoleService = discordRoleService;
            _discordModerationService = discordModerationService;
        }

        public async Task<bool> KickUserDueToRoleAssignmentAsync(
            ulong guildId,
            ulong userId,
            DiscordRoleDto roleToKick,
            string reason
        )
        {
            if (!await _discordRoleService.DoesMemberHaveRoleAsync(userId, guildId, roleToKick.RoleId))
            {
                return false;
            }

            return await _discordModerationService.KickMemberAsync(guildId, userId, reason);
        }
    }
}
