using LundBot.Application.Common.Messaging;
using LundBot.Domain.WebsiteTraffic;

namespace LundBot.Application.Features.WebsiteTraffic
{
    public sealed class WebsiteTrafficMessageFactory : IMessageEntityFactory<WebsiteTrafficAnalyticsMessage>
    {
        private int _websiteTrafficAnalyticsChannelId;

        public WebsiteTrafficAnalyticsMessage Create(ulong discordMessageId)
        {
            return new WebsiteTrafficAnalyticsMessage
            {
                DiscordMessageId = discordMessageId,
                WebsiteTrafficAnalyticsChannelId = _websiteTrafficAnalyticsChannelId,
            };
        }

        public void SetWebsiteTrafficAnalyticsChannelId(int websiteTrafficAnalyticsChannelId)
        {
            _websiteTrafficAnalyticsChannelId = websiteTrafficAnalyticsChannelId;
        }
    }
}
