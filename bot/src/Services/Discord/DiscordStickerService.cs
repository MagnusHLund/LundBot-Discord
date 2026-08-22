using DSharpPlus.Entities;
using LundBot.Interfaces.Services.Discord;

namespace LundBot.Services.Discord
{
    public class DiscordStickerService : IDiscordStickerService
    {
        public Task<IReadOnlyList<DiscordMessageStickerPack>> GetStickerPacksAsync()
        {
            return BotService.DiscordClient.GetStickerPacksAsync();
        }

        public async Task<DiscordMessageStickerPack?> GetStickerPackByNameAsync(string name)
        {
            var stickerPacks = await GetStickerPacksAsync();
            return stickerPacks.FirstOrDefault(sp => sp.Name == name);
        }
    }
}
