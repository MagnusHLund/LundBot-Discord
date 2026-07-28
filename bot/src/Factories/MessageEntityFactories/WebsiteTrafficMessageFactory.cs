using LundBot.Entities;

namespace LundBot.Factories.MessageEntityFactories
{
    public class WebsiteTrafficMessageFactory : IMessageEntityFactory<WebsiteTrafficMessagesEntity>
    {
        public WebsiteTrafficMessagesEntity Create(string discordMessageId)
        {
            return new WebsiteTrafficMessagesEntity { DiscordMessageId = discordMessageId };
        }
    }
}
