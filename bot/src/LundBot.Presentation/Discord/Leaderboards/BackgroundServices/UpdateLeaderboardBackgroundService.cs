using LundBot.Application.Features.Leaderboards;
using LundBot.Application.Features.Leaderboards.Contracts;

namespace LundBot.Presentation.Discord.Leaderboards.BackgroundServices
{
    public sealed class UpdateLeaderboardBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILeaderboardQueue _leaderboardQueue;
        private readonly ILogger _logger = Log.ForContext<UpdateLeaderboardBackgroundService>();

        public UpdateLeaderboardBackgroundService(IServiceScopeFactory scopeFactory, ILeaderboardQueue leaderboardQueue)
        {
            _scopeFactory = scopeFactory;
            _leaderboardQueue = leaderboardQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.Information($"Starting {nameof(UpdateLeaderboardBackgroundService)}...");

            await foreach (var job in _leaderboardQueue.ReadAllAsync(cancellationToken))
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

                using var scope = _scopeFactory.CreateScope();
                var leaderboardService = scope.ServiceProvider.GetRequiredService<ILeaderboardService>();

                _logger.Information(
                    "Processing leaderboard update job for leaderboard ID {LeaderboardId}...",
                    job.Leaderboard.Id
                );

                try
                {
                    await leaderboardService.UpdateLeaderboardMessageAsync(job.Leaderboard, job.Channel);
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
