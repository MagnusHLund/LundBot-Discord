namespace LundBot.Application.Common.Discord
{
    public sealed record DiscordStickerPackDto
    {
        public ulong StickerPackId { get; }
        public string Name { get; }
        public IReadOnlyList<DiscordStickerDto> Stickers { get; }

        public DiscordStickerPackDto(ulong stickerPackId, string name, IReadOnlyList<DiscordStickerDto> stickers)
        {
            StickerPackId = stickerPackId;
            Name = name;
            Stickers = stickers;
        }
    }
}
