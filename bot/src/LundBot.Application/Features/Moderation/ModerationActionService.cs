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
            AutoKickRole? roleToKick = await _autoKickRolesRepository.GetAutoKickRoleAsync(guildId, userId);

            if (roleToKick is null)
            {
                return false;
            }

            if (!await _discordRoleService.DoesMemberHaveRoleAsync(userId, guildId, roleToKick.RoleId))
            {
                return false;
            }

            return await _discordModerationService.KickMemberAsync(userId, guildId, roleToKick.Reason);
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
