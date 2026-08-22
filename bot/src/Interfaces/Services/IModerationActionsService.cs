using DSharpPlus.Entities;

namespace LundBot.Interfaces.Services
{
    public interface IModerationActionsService
    {
        Task<bool> KickUserAsync(DiscordGuild guild, DiscordMember user, string reason);
        Task<bool> KickUserDueToRoleAssignmentAsync(
            DiscordGuild guild,
            DiscordMember user,
            DiscordRole? roleToKick,
            string reason
        );
    }
}
