namespace LundBot.Application.Features.Leaderboards
{
    public interface ILeaderboardQueue
    {
        void Enqueue(LeaderboardUpdateJob job);
        IAsyncEnumerable<LeaderboardUpdateJob> ReadAllAsync(CancellationToken cancellationToken);
    }
}
