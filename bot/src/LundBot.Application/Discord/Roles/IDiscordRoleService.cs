namespace LundBot.Application.Discord.Roles
{
    public interface IDiscordRoleService
    {
        Task<bool> DoesMemberHaveRoleAsync(ulong memberId, ulong guildId, ulong roleId);
        Task<bool> IsMemberAdministratorAsync(ulong memberId, ulong guildId);
        Task<bool> IsMemberOwnerAsync(ulong memberId, ulong guildId);
        Task<bool> IsMemberABotAsync(ulong memberId, ulong guildId);
    }
}
