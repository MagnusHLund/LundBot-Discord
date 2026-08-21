using LundBot.ValueObjects.Jobs;

namespace LundBot.Interfaces.Queues
{
    public interface ILeaderboardQueue
    {
        void Enqueue(LeaderboardUpdateJob job);
        IAsyncEnumerable<LeaderboardUpdateJob> ReadAllAsync(CancellationToken cancellationToken);
    }
}
