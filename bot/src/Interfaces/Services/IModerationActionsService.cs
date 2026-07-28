using DSharpPlus.Entities;

namespace LundBot.Interfaces.Services
{
    public interface IModerationActionsService
    {
        Task KickUserAsync(DiscordGuild guild, DiscordMember user, string reason);
        Task KickUserDueToRoleAssignmentAsync(
            DiscordGuild guild,
            DiscordMember user,
            DiscordRole? roleToKick,
            string reason
        );
    }
}
