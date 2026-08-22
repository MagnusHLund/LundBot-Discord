using DSharpPlus.Entities;

namespace LundBot.Interfaces.Services.Discord
{
    public interface IDiscordMessageService
    {
        Task<DiscordMessage> GetMessageAsync(DiscordChannel channel, ulong messageId);
        Task<DiscordMessage> SendMessageAsync(
            DiscordChannel channel,
            DiscordMessageBuilder message
        );
        Task<DiscordMessage> ModifyMessageAsync(DiscordMessage message, string content);
        Task DeleteMessageAsync(DiscordMessage message);
        Task<DiscordMessage> SendMessageWithComponentsAsync(
            DiscordChannel channel,
            string content,
            List<DiscordComponent> components
        );
    }
}
