using System.Text;
using LundBot.Application.Discord.Channels;
using LundBot.Application.Discord.Interactions;
using LundBot.Application.Discord.Messages;
using LundBot.Domain.Common;

namespace LundBot.Application.Common.Messaging
{
    public sealed class MessageService<TEntity, TRepository, TFactory> : IMessageService<TEntity, TRepository, TFactory>
        where TRepository : IMessageRepository<TEntity>
        where TEntity : AbstractMessageEntity, new()
        where TFactory : IMessageEntityFactory<TEntity>
    {
        private readonly TRepository _messageRepository;
        private readonly IDiscordChannelService _discordChannelService;
        private readonly IDiscordMessageService _discordMessageService;

        private readonly ILogger _logger = Log.ForContext<MessageService<TEntity, TRepository, TFactory>>();

        public MessageService(
            TRepository messageRepository,
            TFactory messageFactory,
            IDiscordChannelService discordChannelService,
            IDiscordMessageService discordMessageService
        )
        {
            _messageRepository = messageRepository;
            MessageFactory = messageFactory;
            _discordChannelService = discordChannelService;
            _discordMessageService = discordMessageService;
        }

        public TFactory MessageFactory { get; }

        public async Task<bool> SynchronizeDiscordMessagesAsync(
            string message,
            IEnumerable<TEntity> existingMessages,
            ulong channelId
        )
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return true;
            }

            List<string> chunks = SplitMessageIntoChunks(message);
            List<TEntity> existing = existingMessages.ToList();

            DiscordChannelDto? channel = await _discordChannelService.GetChannelAsync(channelId);
            if (channel is null)
            {
                _logger.Error("Channel with ID {ChannelId} not found. Cannot synchronize messages.", channelId);
                return false;
            }

            int sharedLength = Math.Min(existing.Count, chunks.Count);

            await UpdateMessagesAsync(sharedLength, existing, chunks, channel.ChannelId);
            await CreateNewMessagesAsync(chunks, existing, channel.ChannelId);
            await DeleteExtraMessagesAsync(existing, chunks, channel.ChannelId);

            return true;
        }

        public async Task<bool> DeleteMessagesForChannelAsync(IEnumerable<TEntity> existingMessages, ulong channelId)
        {
            List<TEntity> existing = existingMessages.ToList();

            foreach (TEntity message in existing)
            {
                DiscordMessageDto? discordMessage = await _discordMessageService.GetMessageAsync(
                    message.DiscordMessageId,
                    channelId
                );

                if (discordMessage is null)
                {
                    _logger.Warning(
                        "Message with ID {MessageId} not found in channel {ChannelId}. It may have already been deleted.",
                        message.DiscordMessageId,
                        channelId
                    );
                    continue;
                }

                await _discordMessageService.DeleteMessageAsync(discordMessage.MessageId, channelId);
            }

            await _messageRepository.DeleteManyAsync(existing.Select(x => x.Id));
            return true;
        }

        public async Task<bool> DeleteMessageByIdAsync(TEntity message, DiscordChannelDto channel)
        {
            DiscordMessageDto? discordMessage = await _discordMessageService.GetMessageAsync(
                message.DiscordMessageId,
                channel.ChannelId
            );

            if (discordMessage is null)
            {
                return false;
            }

            await _discordMessageService.DeleteMessageAsync(discordMessage.MessageId, channel.ChannelId);
            await _messageRepository.DeleteManyAsync(new[] { message.Id });
            return true;
        }

        public async Task<DiscordMessageDto?> CreateMessageWithComponentsAsync(
            string content,
            DiscordChannelDto channel,
            List<DiscordMessageComponentDto> components
        )
        {
            DiscordMessageDto? discordMessage = await _discordMessageService.SendMessageWithComponentsAsync(
                channel.ChannelId,
                content,
                components
            );

            if (discordMessage is null)
            {
                return null;
            }

            await _messageRepository.CreateAsync(MessageFactory.Create(discordMessage.MessageId));
            return discordMessage;
        }

        public async Task<DiscordMessageDto?> CreateMessageFromDiscordMessageBuilderAsync(
            DiscordMessageBuilderDto messageBuilder,
            ulong channelId,
            bool shouldSaveMessageInDatabase = false
        )
        {
            DiscordMessageDto? discordMessage = await _discordMessageService.SendMessageAsync(
                channelId,
                messageBuilder
            );

            if (discordMessage is null)
            {
                return null;
            }

            if (shouldSaveMessageInDatabase)
            {
                await _messageRepository.CreateAsync(MessageFactory.Create(discordMessage.MessageId));
            }

            return discordMessage;
        }

        private async Task UpdateMessagesAsync(
            int sharedLength,
            List<TEntity> existing,
            List<string> chunks,
            ulong channelId
        )
        {
            for (int i = 0; i < sharedLength; i++)
            {
                TEntity existingMessage = existing[i];
                string newContent = chunks[i];

                try
                {
                    DiscordMessageDto? discordMessage = await _discordMessageService.GetMessageAsync(
                        existingMessage.DiscordMessageId,
                        channelId
                    );

                    if (discordMessage is not null)
                    {
                        await _discordMessageService.ModifyMessageAsync(
                            discordMessage.MessageId,
                            channelId,
                            newContent
                        );

                        continue;
                    }

                    _logger.Warning(
                        "Discord message with ID {MessageId} was not found in channel {ChannelId}. Creating a replacement.",
                        existingMessage.DiscordMessageId,
                        channelId
                    );

                    DiscordMessageDto? replacement = await _discordMessageService.SendMessageAsync(
                        channelId,
                        new DiscordMessageBuilderDto(newContent)
                    );

                    if (replacement is null)
                    {
                        _logger.Warning(
                            "Failed to create a replacement message for Discord message with ID {MessageId} in channel {ChannelId}.",
                            existingMessage.DiscordMessageId,
                            channelId
                        );

                        continue;
                    }

                    existingMessage.DiscordMessageId = replacement.MessageId;

                    await _messageRepository.UpdateAsync(existingMessage);
                }
                catch (Exception ex)
                {
                    _logger.Warning(
                        ex,
                        "Failed to synchronize Discord message with ID {MessageId} in channel {ChannelId}.",
                        existingMessage.DiscordMessageId,
                        channelId
                    );
                }
            }
        }

        private async Task CreateNewMessagesAsync(List<string> chunks, List<TEntity> existing, ulong channelId)
        {
            if (chunks.Count <= existing.Count)
            {
                return;
            }

            for (int i = existing.Count; i < chunks.Count; i++)
            {
                DiscordMessageDto? newMessage = await _discordMessageService.SendMessageAsync(
                    channelId,
                    new DiscordMessageBuilderDto(chunks[i])
                );

                if (newMessage is null)
                {
                    _logger.Warning("Failed to create a new message in channel {ChannelId}.", channelId);
                    continue;
                }

                await _messageRepository.CreateAsync(MessageFactory.Create(newMessage.MessageId));
            }
        }

        private async Task DeleteExtraMessagesAsync(List<TEntity> existing, List<string> chunks, ulong channelId)
        {
            if (existing.Count <= chunks.Count)
            {
                return;
            }

            IEnumerable<TEntity> extras = existing.Skip(chunks.Count);
            await DeleteMessagesForChannelAsync(extras, channelId);
        }

        private static List<string> SplitMessageIntoChunks(string message)
        {
            const int MaxChunkSize = 1900;

            if (message.Length <= MaxChunkSize)
            {
                return new List<string> { message };
            }

            List<string> chunks = new();
            string[] lines = message.Split('\n');
            StringBuilder currentChunk = new();

            foreach (string line in lines)
            {
                string lineWithNewLine = line + "\n";

                if (currentChunk.Length > 0 && currentChunk.Length + lineWithNewLine.Length > MaxChunkSize)
                {
                    chunks.Add(currentChunk.ToString().TrimEnd());
                    currentChunk.Clear();
                    currentChunk.Append(lineWithNewLine);
                    continue;
                }

                if (lineWithNewLine.Length > MaxChunkSize)
                {
                    if (currentChunk.Length > 0)
                    {
                        chunks.Add(currentChunk.ToString().TrimEnd());
                        currentChunk.Clear();
                    }

                    int start = 0;
                    while (start < line.Length)
                    {
                        int length = Math.Min(MaxChunkSize, line.Length - start);
                        chunks.Add(line.Substring(start, length));
                        start += MaxChunkSize;
                    }

                    continue;
                }

                currentChunk.Append(lineWithNewLine);
            }

            if (currentChunk.Length > 0)
            {
                chunks.Add(currentChunk.ToString().TrimEnd());
            }

            return chunks.Count > 0 ? chunks : new List<string> { message };
        }
    }
}
