using LundBot.Entities;
using LundBot.Enums;

namespace LundBot.Interfaces.Repositories
{
    public interface ILeaderboardsRepository
    {
        Task<(bool, LeaderboardsEntity?)> DoesLeaderboardExistAsync(
            string channelId,
            string guildId
        );
        Task<LeaderboardsEntity> CreateLeaderboardAsync(
            string channelId,
            string guildId,
            string title,
            string message,
            LeaderboardType leaderboardType
        );
        Task RemoveLeaderboardAsync(string channelId, string guildId);
        Task<(bool, LeaderboardsEntity?)> DoesInviteLeaderboardExistOnServerAsync(string guildId);
    }
}
