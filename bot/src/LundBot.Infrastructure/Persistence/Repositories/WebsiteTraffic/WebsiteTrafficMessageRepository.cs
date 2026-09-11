using LundBot.Application.Features.WebsiteTraffic;
using LundBot.Domain.WebsiteTraffic;

namespace LundBot.Infrastructure.Persistence.Repositories.WebsiteTraffic
{
    public sealed class WebsiteTrafficMessageRepository : IWebsiteTrafficMessageRepository
    {
        public Task<bool> CreateAsync(WebsiteTrafficAnalyticsMessage entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<WebsiteTrafficAnalyticsMessage>> GetWebsiteTrafficMessagesForPeriodAsync(
            DateTime startDate,
            DateTime endDate
        )
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(WebsiteTrafficAnalyticsMessage entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteManyAsync(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }
    }
}
