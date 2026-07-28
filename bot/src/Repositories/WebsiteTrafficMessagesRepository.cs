using LundBot.Data;
using LundBot.Entities;
using LundBot.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LundBot.Repositories
{
    public class WebsiteTrafficMessagesRepository
        : AbstractMessageRepository<WebsiteTrafficMessagesEntity>,
            IWebsiteTrafficMessagesRepository
    {
        private readonly LundBotDiscordDbContext _context;
        private readonly Serilog.ILogger _logger =
            Log.ForContext<WebsiteTrafficMessagesRepository>();

        public WebsiteTrafficMessagesRepository(LundBotDiscordDbContext context)
        {
            _context = context;
        }

        public async Task<
            List<WebsiteTrafficMessagesEntity>
        > GetWebsiteTrafficMessagesForPeriodAsync(DateTime startDate, DateTime endDate)
        {
            return await _context
                .WebsiteTrafficMessages.Where(w =>
                    w.CreatedAt >= startDate && w.CreatedAt <= endDate
                )
                .ToListAsync();
        }

        public override async Task CreateAsync(WebsiteTrafficMessagesEntity entity)
        {
            try
            {
                _context.WebsiteTrafficMessages.Add(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating WebsiteTrafficMessagesEntity: {Entity}", entity);
                throw new Exception("An error occurred while creating the entity.", ex);
            }
        }

        public override async Task UpdateAsync(WebsiteTrafficMessagesEntity entity)
        {
            try
            {
                _context.WebsiteTrafficMessages.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating WebsiteTrafficMessagesEntity: {Entity}", entity);
                throw new Exception("An error occurred while updating the entity.", ex);
            }
        }

        public override async Task DeleteManyAsync(IEnumerable<int> ids)
        {
            try
            {
                var entitiesToDelete = await _context
                    .WebsiteTrafficMessages.Where(w => ids.Contains(w.Id))
                    .ToListAsync();

                _context.WebsiteTrafficMessages.RemoveRange(entitiesToDelete);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error deleting WebsiteTrafficMessagesEntities with IDs: {Ids}",
                    ids
                );
                throw new Exception("An error occurred while deleting the entities.", ex);
            }
        }
    }
}
