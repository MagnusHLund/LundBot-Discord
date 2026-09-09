namespace LundBot.Application.Discord.Messages
{
    public record DiscordMessageBuilderDto(
        string Content,
        ulong? ReplyToMessageId = null,
        IReadOnlyCollection<ulong>? StickerIds = null
    );
}
