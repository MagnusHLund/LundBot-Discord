namespace LundBot.Application.Discord.Messages
{
    public sealed class DiscordMessageBuilderDto
    {
        public string Content { get; set; }
        public ulong? ReplyToMessageId { get; set; }
        public IReadOnlyList<ulong>? StickerIds { get; set; }

        public DiscordMessageBuilderDto(string content, ulong? replyToMessageId = null, List<ulong>? stickerIds = null)
        {
            Content = content;
            ReplyToMessageId = replyToMessageId;
            StickerIds = stickerIds;
        }
    }
}
