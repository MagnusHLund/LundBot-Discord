namespace LundBot.Application.Discord.Stickers
{
    public interface IDiscordStickerService
    {
        Task<IReadOnlyList<DiscordStickerPackDto>> GetAllStickerPacksAsync();
    }
}
