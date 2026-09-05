using LundBot.Application.Discord.Interactions;

namespace LundBot.Application.Discord.Messages
{
    public interface IDiscordMessageService
    {
        Task<DiscordMessageDto?> GetMessageAsync(ulong messageId, ulong channelId);
        Task<DiscordMessageDto?> SendMessageAsync(ulong channelId, string content);
        Task<DiscordMessageDto?> SendMessageWithComponentsAsync(
            ulong channelId,
            string content,
            IReadOnlyCollection<DiscordMessageComponentDto> components
        );
        Task<DiscordMessageDto?> ModifyMessageAsync(ulong messageId, ulong channelId, string newContent);
        Task<bool> DeleteMessageAsync(ulong messageId, ulong channelId);
    }
}
