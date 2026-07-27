using LundBot.Data;
using LundBot.Entities;
using LundBot.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LundBot.Repositories
{
    public class LeaderboardMessagesRepository
        : AbstractMessageRepository<LeaderboardMessagesEntity>,
            ILeaderboardMessagesRepository
    {
        private readonly LundBotDiscordDbContext _context;
        private readonly Serilog.ILogger _logger = Log.ForContext<LeaderboardMessagesRepository>();

        public LeaderboardMessagesRepository(
            LundBotDiscordDbContext context,
            ILogger<LeaderboardMessagesRepository> logger
        )
        {
            _context = context;
        }

        public override async Task CreateAsync(LeaderboardMessagesEntity entity)
        {
            try
            {
                _context.LeaderboardMessages.Add(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating LeaderboardMessagesEntity: {Entity}", entity);
                throw new Exception("An error occurred while creating the entity.", ex);
            }
        }

        public override async Task UpdateAsync(LeaderboardMessagesEntity entity)
        {
            try
            {
                _context.LeaderboardMessages.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating LeaderboardMessagesEntity: {Entity}", entity);
                throw new Exception("An error occurred while updating the entity.", ex);
            }
        }

        public override async Task DeleteManyAsync(IEnumerable<int> ids)
        {
            try
            {
                var entitiesToDelete = await _context
                    .LeaderboardMessages.Where(l => ids.Contains(l.Id))
                    .ToListAsync();

                _context.LeaderboardMessages.RemoveRange(entitiesToDelete);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error deleting LeaderboardMessagesEntities with IDs: {Ids}",
                    ids
                );
                throw new Exception("An error occurred while deleting the entities.", ex);
            }
        }
    }
}
