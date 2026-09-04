namespace LundBot.Application.Common.Discord
{
    public sealed record DiscordStickerDto
    {
        public ulong StickerId { get; }
        public string Name { get; }

        public DiscordStickerDto(ulong stickerId, string name)
        {
            StickerId = stickerId;
            Name = name;
        }
    }
}
