namespace LundBot.Application.Common.Discord
{
    public interface IDiscordUserService
    {
        Task<DiscordUserDto?> GetUserAsync(ulong userId);
    }
}
