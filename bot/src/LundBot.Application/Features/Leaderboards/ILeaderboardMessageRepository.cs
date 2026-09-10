using LundBot.Application.Common.Messaging;
using LundBot.Domain.Leaderboards;

namespace LundBot.Application.Features.Leaderboards
{
    public interface ILeaderboardMessageRepository : IMessageRepository<LeaderboardMessage>
    {
        Task<List<LeaderboardMessage>> GetMessagesForLeaderboardAsync(int leaderboardId);
    }
}
