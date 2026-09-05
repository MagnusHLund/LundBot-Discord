namespace LundBot.Application.Discord.Channels
{
    public interface IDiscordChannelService
    {
        Task<DiscordChannelDto?> GetChannelAsync(ulong channelId, ulong guildId);
        Task<DiscordChannelDto?> GetSystemChannelAsync(ulong guildId);
    }
}
