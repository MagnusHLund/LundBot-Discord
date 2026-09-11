using LundBot.Application.Common.Messaging;
using LundBot.Domain.WebsiteTraffic;

namespace LundBot.Application.Features.WebsiteTraffic
{
    public sealed class WebsiteTrafficMessageFactory : IMessageEntityFactory<WebsiteTrafficAnalyticsMessage>
    {
        public WebsiteTrafficAnalyticsMessage Create(ulong discordMessageId)
        {
            return new WebsiteTrafficAnalyticsMessage { DiscordMessageId = discordMessageId };
        }
    }
}
