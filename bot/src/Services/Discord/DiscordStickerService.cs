using DSharpPlus.Entities;
using LundBot.Interfaces.Services.Discord;

namespace LundBot.Services.Discord
{
    public class DiscordStickerService : IDiscordStickerService
    {
        private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<DiscordStickerService>();

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
