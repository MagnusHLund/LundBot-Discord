using LundBot.Application.Common.Messaging;
using LundBot.Domain.MemberJoin;

namespace LundBot.Application.Features.MemberJoin
{
    public class MemberJoinMessageFactory : IMessageEntityFactory<MemberJoinMessage>
    {
        private ulong _joinedUserId;

        public MemberJoinMessage Create(ulong discordMessageId)
        {
            throw new NotImplementedException();
        }

        public void SetJoinedUserId(ulong joinedUserId)
        {
            _joinedUserId = joinedUserId;
        }
    }
}
