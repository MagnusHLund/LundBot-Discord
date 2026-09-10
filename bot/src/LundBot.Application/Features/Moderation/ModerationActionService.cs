namespace LundBot.Application.Features.Moderation
{
    public sealed class ModerationActionService : IModerationActionService
    {
        public Task<bool> KickUserDueToRoleAssignmentAsync(ulong guildId, ulong userId)
        {
            throw new NotImplementedException();
        }
    }
}
