using LundBot.Application.Features.Leaderboards;

namespace LundBot.Infrastructure.Persistence.Repositories.Leaderboards
{
    public sealed class LeaderboardScoreSourceRepository : ILeaderboardScoreSourceRepository
    {
        public Task<bool> HasUserGivenScoreToTargetAsync(ulong userId, ulong targetUserId, int leaderboardId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddScoreAsync(ulong userId, ulong targetUserId, int leaderboardId)
        {
            throw new NotImplementedException();
        }
    }
}
