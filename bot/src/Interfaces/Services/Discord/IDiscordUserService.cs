using DSharpPlus.Entities;

namespace LundBot.Interfaces.Services.Discord
{
    public interface IDiscordUserService
    {
        Task<DiscordUser> GetUserAsync(ulong userId);
    }
}
