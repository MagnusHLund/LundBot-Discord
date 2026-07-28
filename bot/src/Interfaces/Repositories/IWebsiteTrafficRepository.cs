using LundBot.Entities;

namespace LundBot.Interfaces.Repositories
{
    public interface IWebsiteTrafficRepository
    {
        Task<bool> RegisterWebsiteVisitAsync(byte[] hashedIpAddress);
        Task<bool> RegisterInviteLinkClickAsync(byte[] hashedIpAddress);
        Task<List<WebsiteTrafficEntity>> GetWebsiteTrafficEntitiesForPeriodAsync(
            DateTime startDate,
            DateTime endDate
        );
    }
}
