using LundBot.Application.Features.Leaderboards.Shared;

namespace LundBot.Application.Features.Leaderboards.Contracts
{
    public interface ILeaderboardQueue
    {
        void Enqueue(LeaderboardUpdateJob job);
        IAsyncEnumerable<LeaderboardUpdateJob> ReadAllAsync(CancellationToken cancellationToken);
    }
}
