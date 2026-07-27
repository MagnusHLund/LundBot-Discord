using LundBot.Entities;
using LundBot.Enums;

namespace LundBot.Interfaces.Repositories
{
    public interface ILeaderboardsRepository
    {
        Task<bool> DoesLeaderboardExistAsync(string channelId, string guildId);
        Task<LeaderboardsEntity> CreateLeaderboardAsync(
            string channelId,
            string guildId,
            string title,
            string message,
            LeaderboardType leaderboardType
        );
    }
}
