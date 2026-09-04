namespace LundBot.Application.Common.Discord
{
    public sealed record DiscordChannelDto
    {
        public ulong ChannelId { get; }

        public DiscordChannelDto(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}
