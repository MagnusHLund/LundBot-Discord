using LundBot.Application.Discord.Moderation;
using LundBot.Application.Discord.Roles;
using LundBot.Domain.Moderation;

namespace LundBot.Application.Features.Moderation
{
    public sealed class ModerationActionService : IModerationActionService
    {
        private readonly IDiscordRoleService _discordRoleService;
        private readonly IDiscordModerationService _discordModerationService;
        private readonly IAutoKickRolesRepository _autoKickRolesRepository;

        public ModerationActionService(
            IDiscordRoleService discordRoleService,
            IDiscordModerationService discordModerationService,
            IAutoKickRolesRepository autoKickRolesRepository
        )
        {
            _discordRoleService = discordRoleService;
            _discordModerationService = discordModerationService;
            _autoKickRolesRepository = autoKickRolesRepository;
        }

        public async Task<bool> KickUserDueToRoleAssignmentAsync(ulong guildId, ulong userId)
        {
            IReadOnlyList<AutoKickRole> rolesToKick = await _autoKickRolesRepository.GetAutoKickRoleAsync(guildId);

            if (rolesToKick is null || rolesToKick.Count == 0)
            {
                return false;
            }

            foreach (AutoKickRole role in rolesToKick)
            {
                if (role != null && await _discordRoleService.DoesMemberHaveRoleAsync(userId, guildId, role.RoleId))
                {
                    return await _discordModerationService.KickMemberAsync(userId, guildId, role.Reason);
                }
            }

            return false;
        }

        public async Task<bool> SetRoleToAutomaticallyKickAsync(ulong guildId, ulong roleId, string reason)
        {
            return await _autoKickRolesRepository.AddAutoKickRoleAsync(guildId, roleId, reason);
        }

        public async Task<bool> RemoveRoleFromAutomaticallyKickAsync(ulong guildId, ulong roleId)
        {
            return await _autoKickRolesRepository.RemoveAutoKickRoleAsync(guildId, roleId);
        }
    }
}
