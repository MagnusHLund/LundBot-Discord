namespace LundBot.Interfaces.Services
{
    public interface IUserService
    {
        Task<bool> IsUserAdminAsync(ulong userId, ulong guildId);
        Task<bool> IsUserOwnerAsync(ulong userId, ulong guildId);
    }
}
