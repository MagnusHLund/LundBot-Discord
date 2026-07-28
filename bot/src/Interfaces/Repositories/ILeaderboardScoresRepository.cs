using LundBot.Entities;

namespace LundBot.Interfaces.Repositories
{
    public interface ILeaderboardScoresRepository
    {
        Task IncrementScoreAsync(string userId, int leaderboardId);
        Task<IEnumerable<LeaderboardScoresEntity>> GetTopScoresAsync(int leaderboardId, int limit);
    }
}
