using System.Threading.Channels;
using LundBot.Interfaces.Queues;
using LundBot.ValueObjects.Jobs;

namespace LundBot.Queues
{
    public sealed class LeaderboardQueue : ILeaderboardQueue
    {
        private readonly Channel<LeaderboardUpdateJob> _channel =
            Channel.CreateUnbounded<LeaderboardUpdateJob>();

        public void Enqueue(LeaderboardUpdateJob job)
        {
            _channel.Writer.TryWrite(job);
        }

        public IAsyncEnumerable<LeaderboardUpdateJob> ReadAllAsync(CancellationToken cancellationToken)
        {
            return _channel.Reader.ReadAllAsync(cancellationToken);
        }
    }
}
