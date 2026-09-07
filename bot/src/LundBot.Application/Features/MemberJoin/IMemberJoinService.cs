namespace LundBot.Application.Features.MemberJoin
{
    public interface IMemberJoinService
    {
        Task<bool> HandleMemberJoinHiEventAsync(ulong senderUserId, ulong targetUserId, ulong channelId, ulong guildId);
    }
}
