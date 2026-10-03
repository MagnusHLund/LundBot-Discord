using LundBot.Application.Common.Messaging;
using LundBot.Domain.MemberJoin;

namespace LundBot.Application.Features.MemberJoin
{
    public sealed class MemberJoinMessageFactory : IMessageEntityFactory<MemberJoinMessage>
    {
        private ulong _guildId;
        private ulong _channelId;
        private ulong _joinedUserId;

        public MemberJoinMessage Create(ulong discordMessageId)
        {
            return new MemberJoinMessage(_guildId, _channelId, _joinedUserId, discordMessageId);
        }

        public void SetWelcomeMessageContext(ulong guildId, ulong channelId, ulong joinedUserId)
        {
            _guildId = guildId;
            _channelId = channelId;
            _joinedUserId = joinedUserId;
        }
    }
}
