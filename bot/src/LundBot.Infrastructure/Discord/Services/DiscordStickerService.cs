using DSharpPlus;
using LundBot.Application.Discord.Stickers;
using LundBot.Infrastructure.Discord.Configuration;
using Microsoft.Extensions.Options;
using Serilog;

namespace LundBot.Infrastructure.Discord.Services
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

            return stickers
                .Select(stickerPack => new DiscordStickerPackDto(
                    stickerPackId: stickerPack.Id,
                    name: stickerPack.Name,
                    stickers: stickerPack
                        .Stickers.Select(sticker => new DiscordStickerDto(stickerId: sticker.Id, name: sticker.Name))
                        .ToList()
                ))
                .ToList();
        }
    }
}
