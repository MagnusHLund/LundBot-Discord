using LundBot.Domain.WebsiteTraffic;

namespace LundBot.Application.Features.WebsiteTraffic
{
    public interface IWebsiteTrafficChannelRepository
    {
        Task<WebsiteTrafficAnalyticsChannel?> CreateWebsiteTrafficChannelAsync(ulong channelId, ulong guildId);
        Task<WebsiteTrafficAnalyticsChannel?> GetWebsiteTrafficChannelAsync(ulong guildId);
        Task<bool> RemoveWebsiteTrafficChannelAsync(ulong guildId);
    }
}
