using LundBot.Interfaces.Queues;
using LundBot.Interfaces.Services;
using Serilog;

namespace LundBot.BackgroundServices
{
    // TODO: Does this not need a scope provider?

    public sealed class UpdateLeaderboardBackgroundService : BackgroundService
    {
        private readonly ILeaderboardQueue leaderboardQueue;
        private readonly ILeaderboardService _leaderboardService;
        private readonly Serilog.ILogger _logger =
            Log.ForContext<UpdateLeaderboardBackgroundService>();

        public UpdateLeaderboardBackgroundService(
            ILeaderboardQueue leaderboardQueue,
            ILeaderboardService leaderboardService
        )
        {
            this.leaderboardQueue = leaderboardQueue;
            _leaderboardService = leaderboardService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Starting UpdateLeaderboardBackgroundService...");

            await foreach (var job in leaderboardQueue.ReadAllAsync(cancellationToken))
            {
                if (job.Leaderboard is null)
                {
                    _logger.Warning("Skipping job with null leaderboard.");
                    continue;
                }

                if (job.Channel is null)
                {
                    _logger.Warning("Skipping job with null channel.");
                    continue;
                }

                try
                {
                    _logger.Information(
                        "Processing leaderboard update job for leaderboard ID {LeaderboardId}...",
                        job.Leaderboard.Id
                    );

                    await _leaderboardService.UpdateLeaderboardMessageAsync(
                        job.Leaderboard,
                        job.Channel
                    );
                }
                catch (Exception ex)
                {
                    _logger.Error(
                        ex,
                        "Error processing leaderboard update job for leaderboard ID {LeaderboardId}.",
                        job.Leaderboard.Id
                    );
                }
            }
        }
    }
}
