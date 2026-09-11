using LundBot.Application.Common.Messaging;
using LundBot.Domain.WebsiteTraffic;

namespace LundBot.Application.Features.WebsiteTraffic
{
    public interface IWebsiteTrafficMessageRepository : IMessageRepository<WebsiteTrafficAnalyticsMessage>
    {
        Task<List<WebsiteTrafficAnalyticsMessage>> GetWebsiteTrafficMessagesForPeriodAsync(
            DateTime startDate,
            DateTime endDate
        );
    }
}
