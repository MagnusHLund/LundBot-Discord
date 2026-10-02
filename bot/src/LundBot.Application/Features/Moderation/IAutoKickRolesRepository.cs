using LundBot.Domain.Moderation;

namespace LundBot.Application.Features.Moderation
{
    public interface IAutoKickRolesRepository
    {
        Task<bool> AddAutoKickRoleAsync(ulong guildId, ulong roleId, string reason);
        Task<bool> RemoveAutoKickRoleAsync(ulong guildId, ulong roleId);
        Task<AutoKickRole?> GetAutoKickRoleAsync(ulong guildId, ulong roleId);
    }
}
