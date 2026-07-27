using LundBot.Entities;

namespace LundBot.Factories.MessageEntityFactories
{
    public class LeaderboardMessageFactory : IMessageEntityFactory<LeaderboardMessagesEntity>
    {
        private int _leaderboardId;

        public LeaderboardMessagesEntity Create(string discordMessageId)
        {
            return new LeaderboardMessagesEntity
            {
                DiscordMessageId = discordMessageId,
                LeaderboardsId = _leaderboardId,
            };
        }

        public void SetLeaderboardId(int leaderboardId)
        {
            _leaderboardId = leaderboardId;
        }
    }
}
