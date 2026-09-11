using LundBot.Application.Common.Exceptions;
using LundBot.Application.Features.MemberJoin;
using LundBot.Domain.MemberJoin;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace LundBot.Infrastructure.Persistence.Repositories.MemberJoin
{
    public sealed class MemberJoinMessageRepository : IMemberJoinMessageRepository
    {
        private readonly LundBotDbContext _context;
        private readonly ILogger _logger = Log.ForContext<MemberJoinMessageRepository>();

        public MemberJoinMessageRepository(LundBotDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAsync(MemberJoinMessage entity)
        {
            try
            {
                var existingEntity = await _context.MemberJoinMessages.SingleOrDefaultAsync(wm =>
                    wm.DiscordUserId == entity.DiscordUserId
                );

                if (existingEntity is not null)
                {
                    existingEntity.DiscordMessageId = entity.DiscordMessageId;
                    await _context.SaveChangesAsync();
                    return true;
                }

                _context.MemberJoinMessages.Add(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                _context.Entry(entity).State = EntityState.Detached;

                return await UpdateExistingByDiscordUserIdAsync(entity, ex);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating MemberJoinMessage: {Entity}", entity);
                return false;
            }
        }

        private async Task<bool> UpdateExistingByDiscordUserIdAsync(MemberJoinMessage entity, Exception originalEx)
        {
            try
            {
                var existingEntity = await _context.MemberJoinMessages.SingleOrDefaultAsync(wm =>
                    wm.DiscordUserId == entity.DiscordUserId
                );

                if (existingEntity is null)
                {
                    _logger.Error(
                        originalEx,
                        "Unique-key conflict creating MemberJoinMessage for DiscordUserId {DiscordUserId}, "
                            + "but no existing row was found to update.",
                        entity.DiscordUserId
                    );

                    return false;
                }

                existingEntity.DiscordMessageId = entity.DiscordMessageId;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating MemberJoinMessage after unique-key conflict: {Entity}", entity);
                return false;
            }
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException ex) =>
            ex.InnerException is MySqlException { Number: 1062 };

        public async Task<bool> DeleteManyAsync(IEnumerable<int> ids)
        {
            try
            {
                List<MemberJoinMessage> entitiesToDelete = await _context
                    .MemberJoinMessages.Where(e => ids.Contains(e.Id))
                    .ToListAsync();

                _context.MemberJoinMessages.RemoveRange(entitiesToDelete);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting MemberJoinMessages with IDs: {Ids}", ids);
                return false;
            }
        }

        public async Task<MemberJoinMessage?> GetByJoinedUserIdAsync(ulong joinedUserId)
        {
            try
            {
                return await _context.MemberJoinMessages.FirstOrDefaultAsync(e => e.DiscordUserId == joinedUserId)
                    ?? throw new KeyNotFoundException($"No welcome message found for DiscordUserId {joinedUserId}.");
            }
            catch (KeyNotFoundException)
            {
                _logger.Warning("No MemberJoinMessage found for DiscordUserId: {DiscordUserId}", joinedUserId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error retrieving MemberJoinMessage for DiscordUserId: {DiscordUserId}",
                    joinedUserId
                );

                throw new RepositoryException(
                    $"Failed to retrieve the welcome message for Discord user ID {joinedUserId}.",
                    ex
                );
            }
        }

        public async Task<bool> UpdateAsync(MemberJoinMessage entity)
        {
            try
            {
                _context.MemberJoinMessages.Update(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating MemberJoinMessage: {Entity}", entity);
                return false;
            }
        }
    }
}
