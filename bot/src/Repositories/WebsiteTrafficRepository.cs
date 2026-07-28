using LundBot.Data;
using LundBot.Entities;
using LundBot.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LundBot.Repositories
{
    public class WebsiteTrafficRepository : IWebsiteTrafficRepository
    {
        private readonly LundBotDiscordDbContext _context;
        private readonly Serilog.ILogger _logger = Log.ForContext<WebsiteTrafficRepository>();

        public WebsiteTrafficRepository(LundBotDiscordDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterWebsiteVisitAsync(byte[] hashedIpAddress)
        {
            var websiteVisit = new WebsiteTrafficEntity
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

        public async Task<List<WebsiteTrafficEntity>> GetWebsiteTrafficEntitiesForPeriodAsync(
            DateTime startDate,
            DateTime endDate
        )
        {
            return await _context
                .WebsiteTraffic.Where(w => w.CreatedAt >= startDate && w.CreatedAt <= endDate)
                .ToListAsync();
        }
    }
}
