namespace LundBot.Application.Common.Discord
{
    public interface IDiscordMemberService
    {
        Task<DiscordMemberDto?> GetMemberAsync(ulong memberId, ulong guildId);
        Task<bool> DoesMemberHavePermissionAsync(ulong memberId, ulong guildId, DiscordPermissionEnum permission);
        Task<bool> PreloadMembersAsync(ulong guildId);
    }
}
