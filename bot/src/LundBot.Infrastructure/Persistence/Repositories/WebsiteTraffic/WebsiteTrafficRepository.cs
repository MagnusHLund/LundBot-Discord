using LundBot.Application.Features.WebsiteTraffic;
using LundBot.Domain.WebsiteTraffic;
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
            DateTime endDate
        )
        {
            return await _context
                .WebsiteTraffic.Where(w => w.CreatedAt >= startDate && w.CreatedAt <= endDate)
                .ToListAsync();
        }

        public async Task<bool> RegisterInviteLinkClickAsync(byte[] hashedIpAddress)
        {
            var websiteVisit = await _context
                .WebsiteTraffic.Where(w => w.HashedIp == hashedIpAddress)
                .FirstOrDefaultAsync();

            if (websiteVisit == null)
            {
                _logger.Warning(
                    "No website visit found for hashed IP: {HashedIp}. Cannot register invite link click.",
                    Convert.ToBase64String(hashedIpAddress)
                );
                return false;
            }

            try
            {
                websiteVisit.ClickedInviteButton = true;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error registering invite link click for hashed IP: {HashedIp}",
                    Convert.ToBase64String(hashedIpAddress)
                );
                return false;
            }

            return true;
        }

        public async Task<bool> RegisterWebsiteVisitAsync(byte[] hashedIpAddress)
        {
            WebsiteTrafficAnalytics websiteVisit = new WebsiteTrafficAnalytics
            {
                HashedIp = hashedIpAddress,
                ClickedInviteButton = false,
            };

            try
            {
                _context.WebsiteTraffic.Add(websiteVisit);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error registering website visit for hashed IP: {HashedIp}",
                    Convert.ToBase64String(hashedIpAddress)
                );
                return false;
            }

            return true;
        }
    }
}
