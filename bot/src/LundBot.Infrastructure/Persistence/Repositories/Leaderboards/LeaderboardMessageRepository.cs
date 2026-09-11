using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Domain.Leaderboards;
using Microsoft.EntityFrameworkCore;

namespace LundBot.Infrastructure.Persistence.Repositories.Leaderboards
{
    public sealed class LeaderboardMessageRepository : ILeaderboardMessageRepository
    {
        private readonly LundBotDbContext _context;
        private readonly ILogger _logger = Log.ForContext<LeaderboardMessageRepository>();

        public LeaderboardMessageRepository(LundBotDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAsync(LeaderboardMessage entity)
        {
            try
            {
                _context.LeaderboardMessages.Add(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating LeaderboardMessagesEntity: {Entity}", entity);
                return false;
            }
        }

        public async Task<List<LeaderboardMessage>> GetMessagesForLeaderboardAsync(int leaderboardId)
        {
            try
            {
                return await _context.LeaderboardMessages.Where(l => l.LeaderboardId == leaderboardId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error retrieving LeaderboardMessagesEntities for leaderboard ID: {LeaderboardId}",
                    leaderboardId
                );
                return new List<LeaderboardMessage>();
            }
        }

        public async Task<bool> UpdateAsync(LeaderboardMessage entity)
        {
            try
            {
                _context.LeaderboardMessages.Update(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating LeaderboardMessagesEntity: {Entity}", entity);
                return false;
            }
        }

        public async Task<bool> DeleteManyAsync(IEnumerable<int> ids)
        {
            try
            {
                List<LeaderboardMessage> entitiesToDelete = await _context
                    .LeaderboardMessages.Where(l => ids.Contains(l.Id))
                    .ToListAsync();

                _context.LeaderboardMessages.RemoveRange(entitiesToDelete);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting LeaderboardMessagesEntities with IDs: {Ids}", ids);
                return false;
            }
        }
    }
}
