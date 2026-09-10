using LundBot.Application.Common.Messaging;
using LundBot.Domain.MemberJoin;

namespace LundBot.Application.Features.MemberJoin
{
    public interface IMemberJoinMessageRepository : IMessageRepository<MemberJoinMessage>
    {
        Task<MemberJoinMessage?> GetByJoinedUserIdAsync(ulong joinedUserId);
    }
}
