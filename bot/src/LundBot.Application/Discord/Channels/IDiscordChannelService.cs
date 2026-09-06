namespace LundBot.Application.Discord.Channels
{
    public interface IDiscordChannelService
    {
        Task<DiscordChannelDto?> GetChannelAsync(ulong channelId);
        Task<DiscordChannelDto?> GetSystemChannelAsync(ulong guildId);
    }
}
