using LundBot.Application.Common.Messaging;
using LundBot.Domain.MemberJoin;

namespace LundBot.Application.Features.MemberJoin
{
    public sealed class MemberJoinMessageFactory : IMessageEntityFactory<MemberJoinMessage>
    {
        private ulong _joinedUserId;

        public MemberJoinMessage Create(ulong discordMessageId)
        {
            return new MemberJoinMessage(_joinedUserId, discordMessageId);
        }

        public void SetJoinedUserId(ulong joinedUserId)
        {
            _joinedUserId = joinedUserId;
        }
    }
}
