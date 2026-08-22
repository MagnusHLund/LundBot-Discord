using DSharpPlus.Entities;

namespace LundBot.Interfaces.Services.Discord
{
    public interface IDiscordStickerService
    {
        Task<IReadOnlyList<DiscordMessageStickerPack>> GetStickerPacksAsync();
        Task<DiscordMessageStickerPack?> GetStickerPackByNameAsync(string name);
    }
}
