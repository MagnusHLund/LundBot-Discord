namespace LundBot.Application.Features.Leaderboards.Contracts
{
    public interface IWarnLeaderboardService : IAbstractLeaderboardService
    {
        Task<bool> RegisterWarningAsync(ulong channelId, ulong senderUserId, ulong targetUserId);
    }
}
