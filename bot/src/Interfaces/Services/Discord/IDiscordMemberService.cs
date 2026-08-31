using DSharpPlus.Entities;

namespace LundBot.Interfaces.Services.Discord
{
    public interface IDiscordMemberService
    {
        Task<DiscordMember> GetMemberAsync(DiscordGuild guild, ulong userId);
        bool MemberHasRole(DiscordMember member, DiscordRole role);
        Task KickMemberAsync(DiscordMember member, string reason);
        Task<bool> IsMemberAdminInGuildAsync(DiscordMember member, DiscordGuild guild);
        Task<bool> MemberHasPermission(DiscordMember member, DiscordPermission permission);
        Task<bool> IsMemberOwnerInGuildAsync(DiscordMember member, DiscordGuild guild);
        Task PreloadMembersAsync(DiscordGuild guild);
    }
}
