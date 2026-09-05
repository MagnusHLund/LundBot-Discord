using DSharpPlus;
using LundBot.Application.Discord.Stickers;
using LundBot.Infrastructure.Discord.Stickers.Mappings;
using Serilog;

namespace LundBot.Infrastructure.Discord.Stickers
{
    public sealed class DiscordStickerService : IDiscordStickerService
    {
        private readonly DiscordClient _discordClient;

        private readonly ILogger _logger = Log.ForContext<DiscordStickerService>();

        public DiscordStickerService(DiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        public async Task<IReadOnlyList<DiscordStickerPackDto>> GetAllStickerPacksAsync()
        {
            _logger.Information("Fetching all sticker packs...");

            var stickers = await _discordClient.GetStickerPacksAsync();
            return stickers.Select(DiscordStickerMapper.Map).ToList();
        }
    }
}
