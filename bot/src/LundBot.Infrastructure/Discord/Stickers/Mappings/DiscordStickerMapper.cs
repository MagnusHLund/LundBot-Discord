using DSharpPlus.Entities;
using LundBot.Application.Discord.Stickers;

namespace LundBot.Infrastructure.Discord.Stickers.Mappings
{
    public static class DiscordStickerMapper
    {
        public static DiscordStickerPackDto Map(DiscordMessageStickerPack stickerPack)
        {
            return new DiscordStickerPackDto(
                stickerPackId: stickerPack.Id,
                name: stickerPack.Name,
                stickers: stickerPack.Stickers.Select(sp => Map(sp)).ToList()
            );
        }

        public static DiscordStickerDto Map(DiscordMessageSticker sticker)
        {
            return new DiscordStickerDto(stickerId: sticker.Id, name: sticker.Name);
        }
    }
}
