using DSharpPlus.Entities;

namespace LundBot.Interfaces.Services.Discord
{
    public interface IDiscordMessageService
    {
        Task<DiscordMessage> GetMessageAsync(DiscordChannel channel, ulong messageId);
        Task<DiscordMessage> SendMessageAsync(DiscordChannel channel, string content);
        Task<DiscordMessage> ModifyMessageAsync(DiscordMessage message, string content);
        Task DeleteMessageAsync(DiscordMessage message);
    }
}
