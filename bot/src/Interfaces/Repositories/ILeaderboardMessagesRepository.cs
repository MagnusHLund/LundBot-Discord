using LundBot.Entities;

namespace LundBot.Interfaces.Repositories
{
    public interface ILeaderboardMessagesRepository
    {
        Task<List<LeaderboardMessagesEntity>> GetMessagesForLeaderboardAsync(int leaderboardId);
    }
}
