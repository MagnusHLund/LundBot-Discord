using LundBot.Entities;

namespace LundBot.Interfaces.Repositories
{
    public interface IWebsiteTrafficMessagesRepository
    {
        Task<List<WebsiteTrafficMessagesEntity>> GetWebsiteTrafficMessagesForPeriodAsync(
            DateTime startDate,
            DateTime endDate
        );

        Task CreateAsync(WebsiteTrafficMessagesEntity entity);
        Task UpdateAsync(WebsiteTrafficMessagesEntity entity);
        Task DeleteManyAsync(IEnumerable<int> ids);
    }
}
