using LundBot.Application.Features.MemberJoin;
using LundBot.Domain.MemberJoin;

namespace LundBot.Infrastructure.Persistence.Repositories.MemberJoin
{
    public sealed class MemberJoinMessageRepository : IMemberJoinMessageRepository
    {
        public Task<bool> CreateAsync(MemberJoinMessage entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteManyAsync(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public Task<MemberJoinMessage?> GetByJoinedUserIdAsync(ulong joinedUserId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(MemberJoinMessage entity)
        {
            throw new NotImplementedException();
        }
    }
}
