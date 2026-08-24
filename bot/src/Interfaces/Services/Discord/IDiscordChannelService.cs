using DSharpPlus.Entities;

namespace LundBot.Interfaces.Services.Discord
{
    public interface IDiscordChannelService
    {
        Task<DiscordChannel> GetChannelAsync(ulong channelId);
        Task<DiscordChannel> GetSystemChannelAsync(DiscordGuild guild);
    }
}
