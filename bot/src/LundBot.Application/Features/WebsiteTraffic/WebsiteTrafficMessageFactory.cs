using LundBot.Application.Common.Messaging;
using LundBot.Domain.WebsiteTraffic;

namespace LundBot.Application.Features.WebsiteTraffic
{
    public sealed class WebsiteTrafficMessageFactory : IMessageEntityFactory<WebsiteTrafficMessage>
    {
        public WebsiteTrafficMessage Create(ulong discordMessageId)
        {
            return new WebsiteTrafficMessage { DiscordMessageId = discordMessageId };
        }
    }
}
