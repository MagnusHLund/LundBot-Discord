using LundBot.Application.Features.WebsiteTraffic;
using LundBot.Domain.WebsiteTraffic;

namespace LundBot.Infrastructure.Persistence.Repositories.WebsiteTraffic
{
    public sealed class WebsiteTrafficRepository : IWebsiteTrafficRepository
    {
        public Task<List<WebsiteTrafficAnalytics>> GetWebsiteTrafficEntitiesForPeriodAsync(
            DateTime startDate,
            DateTime endDate
        )
        {
            throw new NotImplementedException();
        }

        public Task<bool> RegisterInviteLinkClickAsync(byte[] hashedIpAddress)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RegisterWebsiteVisitAsync(byte[] hashedIpAddress)
        {
            throw new NotImplementedException();
        }
    }
}
