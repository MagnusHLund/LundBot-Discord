namespace LundBot.Application.Common.Discord
{
    public interface IDiscordRoleService
    {
        Task<bool> DoesMemberHaveRoleAsync(ulong memberId, ulong guildId, ulong roleId);
    }
}
