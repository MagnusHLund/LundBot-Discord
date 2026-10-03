using LundBot.Application.Features.Moderation;
using LundBot.Domain.Moderation;
using Microsoft.EntityFrameworkCore;

namespace LundBot.Infrastructure.Persistence.Repositories.Moderation
{
    public sealed class AutoKickRolesRepository : IAutoKickRolesRepository
    {
        private readonly LundBotDbContext _context;

        private readonly ILogger _logger = Log.ForContext<AutoKickRolesRepository>();

        public AutoKickRolesRepository(LundBotDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAutoKickRoleAsync(ulong guildId, ulong roleId, string reason)
        {
            try
            {
                AutoKickRole autoKickRole = new AutoKickRole(roleId, guildId, reason);

                _context.AutoKickRoles.Add(autoKickRole);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to add auto kick role for guild {GuildId} and role {RoleId}",
                    guildId,
                    roleId
                );
                return false;
            }
        }

        public async Task<List<AutoKickRole>> GetAutoKickRoleAsync(ulong guildId)
        {
            try
            {
                return await _context.AutoKickRoles.Where(x => x.GuildId == guildId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get auto kick roles for guild {GuildId}", guildId);
                throw;
            }
        }

        public async Task<bool> RemoveAutoKickRoleAsync(ulong guildId, ulong roleId)
        {
            try
            {
                var autoKickRole = await _context.AutoKickRoles.FirstOrDefaultAsync(x =>
                    x.GuildId == guildId && x.RoleId == roleId
                );

                if (autoKickRole == null)
                {
                    return false;
                }

                _context.AutoKickRoles.Remove(autoKickRole);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to remove auto kick role for guild {GuildId} and role {RoleId}",
                    guildId,
                    roleId
                );
                return false;
            }
        }
    }
}
