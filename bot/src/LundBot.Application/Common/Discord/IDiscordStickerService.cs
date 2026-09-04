namespace LundBot.Application.Common.Discord
{
    public interface IDiscordStickerService
    {
        Task<IReadOnlyList<DiscordStickerPackDto>> GetStickerPacksAsync(ulong guildId);
    }
}
