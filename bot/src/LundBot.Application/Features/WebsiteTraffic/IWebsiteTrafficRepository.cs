using LundBot.Domain.WebsiteTraffic;

namespace LundBot.Application.Features.WebsiteTraffic
{
    public interface IWebsiteTrafficRepository
    {
        Task<bool> RegisterWebsiteVisitAsync(byte[] hashedIpAddress, int websiteTrafficAnalyticsChannelId);
        Task<bool> RegisterInviteLinkClickAsync(byte[] hashedIpAddress, int websiteTrafficAnalyticsChannelId);
        Task<List<WebsiteTrafficAnalytics>> GetWebsiteTrafficEntitiesForPeriodAsync(
            DateTime startDate,
            DateTime endDate,
            int websiteTrafficAnalyticsChannelId
        );
    }
}
