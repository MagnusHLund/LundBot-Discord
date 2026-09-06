namespace LundBot.Application.Features.Moderation
{
    public interface IModerationActionService
    {
        Task<bool> KickUserDueToRoleAssignmentAsync(ulong guildId, ulong userId);
    }
}
