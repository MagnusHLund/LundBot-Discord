using DSharpPlus.Entities;
using LundBot.Application.Discord.Members;

namespace LundBot.Infrastructure.Discord.Members.Mappings
{
    public static class DiscordMemberMapper
    {
        public static DiscordMemberDto Map(DiscordMember member)
        {
            return new DiscordMemberDto(userId: member.Id, username: member.Username, displayName: member.DisplayName);
        }
    }
}
