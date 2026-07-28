namespace LundBot.Interfaces.Repositories
{
    public interface IUpvotingLeaderboardRepository
    {
        Task<bool> HasUserUpvotedTargetAsync(string userId, string targetUserId, int leaderboardId);
        Task AddUpvoteAsync(string userId, string targetUserId, int leaderboardId);
    }
}
