using LundBot.Domain.Leaderboards;

namespace LundBot.Application.Features.Leaderboards
{
    public interface ILeaderboardScoreRepository
    {
        Task<bool> IncrementScoreAsync(ulong userId, int leaderboardId);
        Task<IEnumerable<LeaderboardScore>> GetTopScoresAsync(int leaderboardId, int limit);
    }
}
