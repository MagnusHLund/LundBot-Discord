namespace LundBot.Application.Features.Moderation
{
    public interface IModerationActionService
    {
        Task<bool> KickUserDueToRoleAssignmentAsync(ulong guildId, ulong userId);

        Task<bool> SetRoleToAutomaticallyKickAsync(ulong guildId, ulong roleId, string reason);
        Task<bool> RemoveRoleFromAutomaticallyKickAsync(ulong guildId, ulong roleId);
    }
}
