namespace LundBot.Application.Features.Leaderboards.Contracts
{
    public interface IWarnLeaderboardService : ILeaderboardService
    {
        Task<bool> RegisterWarningAsync(ulong channelId, ulong senderUserId, ulong targetUserId);
    }
}
