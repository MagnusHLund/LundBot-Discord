namespace LundBot.Application.Common.Discord
{
    public sealed record DiscordMessageDto
    {
        public ulong MessageId { get; init; }
        public ulong ChannelId { get; init; }
        public ulong AuthorId { get; init; }
        public string Content { get; init; } = string.Empty;

        public DiscordMessageDto(ulong messageId, ulong channelId, ulong authorId, string content)
        {
            MessageId = messageId;
            ChannelId = channelId;
            AuthorId = authorId;
            Content = content;
        }
    }
}
