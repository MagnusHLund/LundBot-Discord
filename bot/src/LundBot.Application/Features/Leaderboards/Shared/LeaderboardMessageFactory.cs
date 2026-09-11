using LundBot.Application.Common.Messaging;
using LundBot.Domain.Leaderboards;

namespace LundBot.Application.Features.Leaderboards.Shared
{
    public sealed class LeaderboardMessageFactory : IMessageEntityFactory<LeaderboardMessage>
    {
        private int _leaderboardId;

        public LeaderboardMessage Create(ulong discordMessageId)
        {
            return new LeaderboardMessage(_leaderboardId, discordMessageId);
        }

        public void SetLeaderboardId(int leaderboardId)
        {
            _leaderboardId = leaderboardId;
        }
    }
}
