namespace LundBot.Application.Features.Leaderboards
{
    public interface ILeaderboardScoreSourceRepository
    {
        Task<bool> HasUserGivenScoreToTargetAsync(ulong userId, ulong targetUserId, int leaderboardId);
        Task<bool> AddScoreAsync(ulong userId, ulong targetUserId, int leaderboardId);
    }
}
