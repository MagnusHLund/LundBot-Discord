using LundBot.Application.Discord.Channels;
using LundBot.Application.Discord.Interactions;
using LundBot.Application.Discord.Messages;
using LundBot.Application.Features.Leaderboards;
using LundBot.Domain.Common;

namespace LundBot.Application.Common.Messaging
{
    public interface IMessageService<TEntity, TRepository, TFactory>
        where TRepository : ILeaderboardMessageRepository
        where TFactory : IMessageEntityFactory<TEntity>
        where TEntity : AbstractMessageEntity, new()
    {
        TFactory MessageFactory { get; }

        Task<bool> SynchronizeDiscordMessagesAsync(
            string message,
            IEnumerable<TEntity> existingMessages,
            ulong channelId
        );

        Task<bool> DeleteMessagesForChannelAsync(IEnumerable<TEntity> existingMessages, ulong channelId);

        Task<bool> DeleteMessageByIdAsync(TEntity message, DiscordChannelDto channel);

        Task<DiscordMessageDto?> CreateMessageWithComponentsAsync(
            string content,
            DiscordChannelDto channel,
            List<DiscordMessageComponentDto> components
        );

        Task<DiscordMessageDto?> CreateMessageFromDiscordMessageBuilderAsync(
            DiscordMessageBuilderDto messageBuilder,
            DiscordChannelDto channel,
            bool shouldSaveMessageInDatabase = false
        );
    }
}
