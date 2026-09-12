using LundBot.Application.Features.WebsiteTraffic;
using LundBot.Domain.WebsiteTraffic;
using Microsoft.EntityFrameworkCore;

namespace LundBot.Infrastructure.Persistence.Repositories.WebsiteTraffic
{
    public sealed class WebsiteTrafficMessageRepository : IWebsiteTrafficMessageRepository
    {
        private readonly LundBotDbContext _context;
        private readonly ILogger _logger = Log.ForContext<WebsiteTrafficMessageRepository>();

        public WebsiteTrafficMessageRepository(LundBotDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAsync(WebsiteTrafficAnalyticsMessage entity)
        {
            try
            {
                _context.WebsiteTrafficMessages.Add(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating WebsiteTrafficMessagesEntity: {Entity}", entity);
                return false;
            }
        }

        public async Task<List<WebsiteTrafficAnalyticsMessage>> GetWebsiteTrafficMessagesForPeriodAsync(
            DateTime startDate,
            DateTime endDate,
            int websiteTrafficAnalyticsChannelId
        )
        {
            return await _context
                .WebsiteTrafficMessages.Where(w =>
                    w.WebsiteTrafficAnalyticsChannelId == websiteTrafficAnalyticsChannelId
                    && w.CreatedAt >= startDate
                    && w.CreatedAt < endDate
                )
                .OrderBy(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(WebsiteTrafficAnalyticsMessage entity)
        {
            try
            {
                _context.WebsiteTrafficMessages.Update(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating WebsiteTrafficMessagesEntity: {Entity}", entity);
                return false;
            }
        }

        public async Task<bool> DeleteManyAsync(IEnumerable<int> ids)
        {
            try
            {
                List<WebsiteTrafficAnalyticsMessage> entitiesToDelete = await _context
                    .WebsiteTrafficMessages.Where(w => ids.Contains(w.Id))
                    .ToListAsync();

                _context.WebsiteTrafficMessages.RemoveRange(entitiesToDelete);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting WebsiteTrafficMessagesEntities with IDs: {Ids}", ids);
                return false;
            }
        }
    }
}
