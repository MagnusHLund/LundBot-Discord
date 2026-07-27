using DSharpPlus;
using LundBot.Entities;

namespace LundBot.Interfaces.Services
{
    public interface IMessageService
    {
        Task SynchronizeWebsiteTrafficMessagesAsync(
            string message,
            IEnumerable<WebsiteTrafficMessagesEntity> existingMessages,
            DiscordClient _discordClient
        );
    }
}
