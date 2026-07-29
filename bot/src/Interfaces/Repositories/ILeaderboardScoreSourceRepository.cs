namespace LundBot.Interfaces.Repositories
{
    public interface ILeaderboardScoreSourceRepository
    {
        Task<bool> HasUserGivenScoreToTargetAsync(
            string userId,
            string targetUserId,
            int leaderboardId
        );
        Task AddScoreAsync(string userId, string targetUserId, int leaderboardId);
    }
}
