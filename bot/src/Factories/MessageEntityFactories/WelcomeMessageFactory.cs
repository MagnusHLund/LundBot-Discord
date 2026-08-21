using LundBot.Entities;

namespace LundBot.Factories.MessageEntityFactories
{
    public sealed class WelcomeMessageFactory : IMessageEntityFactory<WelcomeMessageEntity>
    {
        private string _joinedUserId = string.Empty;

        public WelcomeMessageEntity Create(string discordMessageId)
        {
            return new WelcomeMessageEntity
            {
                DiscordMessageId = discordMessageId,
                DiscordUserId = _joinedUserId,
            };
        }

        public void SetJoinedUserId(string joinedUserId)
        {
            _joinedUserId = joinedUserId;
        }
    }
}
