using LundBot.Application.Discord.Members;

namespace LundBot.Application.Features.MemberJoin
{
    public interface IMemberJoinService
    {
        Task<bool> HandleMemberJoinHiEventAsync(ulong senderUserId, ulong targetUserId, ulong channelId, ulong guildId);
        Task SendWelcomeMessageAsync(ulong guildId, DiscordMemberDto member);
        Task RemoveWelcomeMessageAsync(ulong guildId, ulong discordMemberId);
    }
}
