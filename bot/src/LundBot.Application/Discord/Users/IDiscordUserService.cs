namespace LundBot.Application.Discord.Users
{
    public interface IDiscordUserService
    {
        Task<DiscordUserDto?> GetUserAsync(ulong userId);
    }
}
