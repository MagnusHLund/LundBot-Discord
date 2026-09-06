namespace LundBot.Application.Discord.Roles
{
    public interface IDiscordRoleService
    {
        Task<bool> DoesMemberHaveRoleAsync(ulong memberId, ulong guildId, ulong roleId);
    }
}
