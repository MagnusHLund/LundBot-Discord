using LundBot.Application.Features.WebsiteTraffic;
using LundBot.Domain.WebsiteTraffic;
using LundBot.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace LundBot.Infrastructure.Persistence.Repositories.WebsiteTraffic
{
    public sealed class WebsiteTrafficRepository : IWebsiteTrafficRepository
    {
        private readonly LundBotDbContext _context;
        private readonly ILogger _logger = Log.ForContext<WebsiteTrafficRepository>();

        public WebsiteTrafficRepository(LundBotDbContext context)
        {
            _context = context;
        }

        public async Task<List<WebsiteTrafficAnalytics>> GetWebsiteTrafficEntitiesForPeriodAsync(
            DateTime startDate,
            DateTime endDate,
            int websiteTrafficAnalyticsChannelId
        )
        {
            return await _context
                .WebsiteTraffic.Where(w =>
                    w.WebsiteTrafficAnalyticsChannelId == websiteTrafficAnalyticsChannelId
                    && w.CreatedAt >= startDate
                    && w.CreatedAt < endDate
                )
                .OrderBy(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> RegisterInviteLinkClickAsync(
            byte[] hashedIpAddress,
            int websiteTrafficAnalyticsChannelId
        )
        {
            try
            {
                var websiteVisit = await _context
                    .WebsiteTraffic.Where(w =>
                        w.HashedIp == hashedIpAddress
                        && w.WebsiteTrafficAnalyticsChannelId == websiteTrafficAnalyticsChannelId
                    )
                    .FirstOrDefaultAsync();

                if (websiteVisit == null)
                {
                    _logger.Warning("No website visit found for user. Cannot register invite link click.");
                    return false;
                }

                websiteVisit.ClickedInviteButton = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error registering invite link click for user.");
                return false;
            }
        }

        public async Task<bool> RegisterWebsiteVisitAsync(
            byte[] hashedIpAddress,
            int websiteTrafficAnalyticsChannelId
        )
        {
            WebsiteTrafficAnalytics websiteVisit = new WebsiteTrafficAnalytics
            {
                HashedIp = hashedIpAddress,
                ClickedInviteButton = false,
                WebsiteTrafficAnalyticsChannelId = websiteTrafficAnalyticsChannelId,
            };

            try
            {
                _context.WebsiteTraffic.Add(websiteVisit);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex) when (DatabaseUtils.IsUniqueConstraintViolation(ex))
            {
                _logger.Warning("Ip has already been registered");
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error registering website visit for user.");
                return false;
            }
        }
    }
}
