using LundBot.Application.Features.WebsiteTraffic;
using LundBot.Domain.WebsiteTraffic;

namespace LundBot.Infrastructure.Persistence.Repositories.WebsiteTraffic
{
    public sealed class WebsiteTrafficMessageRepository : IWebsiteTrafficMessageRepository
    {
        public Task<bool> CreateAsync(WebsiteTrafficMessage entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(WebsiteTrafficMessage entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteManyAsync(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }
    }
}
