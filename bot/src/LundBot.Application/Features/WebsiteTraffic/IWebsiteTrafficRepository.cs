using LundBot.Domain.WebsiteTraffic;

namespace LundBot.Application.Features.WebsiteTraffic
{
    public interface IWebsiteTrafficRepository
    {
        Task<bool> RegisterWebsiteVisitAsync(byte[] hashedIpAddress);
        Task<bool> RegisterInviteLinkClickAsync(byte[] hashedIpAddress);
        Task<List<WebsiteTrafficAnalytics>> GetWebsiteTrafficEntitiesForPeriodAsync(
            DateTime startDate,
            DateTime endDate
        );
    }
}
