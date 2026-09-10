using System.Text;
using LundBot.Application.Discord.Channels;
using LundBot.Application.Discord.Interactions;
using LundBot.Application.Discord.Messages;
using LundBot.Domain.Common;

namespace LundBot.Application.Common.Messaging
{
    public sealed class MessageService<TEntity, TRepository, TFactory>
        where TRepository : AbstractMessageRepository<TEntity>
        where TEntity : AbstractMessageEntity, new()
        where TFactory : IMessageEntityFactory<TEntity>, IMessageService<TEntity, TFactory>
    {
        private TFactory MessageFactory { get; set; }
        private readonly IDiscordChannelService _discordChannelService;
        private readonly IDiscordMessageService _discordMessageService;

        private readonly TRepository _messageRepository;

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

        public async Task SynchronizeDiscordMessagesAsync(
            string message,
            IEnumerable<TEntity> existingMessages,
            ulong channelId
        )
        {
            List<string> chunks = SplitMessageIntoChunks(message);
            List<TEntity> existing = existingMessages.ToList();

            DiscordChannelDto? channel = await _discordChannelService.GetChannelAsync(channelId);

            if (channel is null)
            {
                _logger.Error("Channel with ID {ChannelId} not found. Cannot synchronize messages.", channelId);
                return;
            }

            int sharedLength = Math.Min(existing.Count, chunks.Count);

            await UpdateMessagesAsync(sharedLength, existing, chunks, channel.ChannelId);
            await CreateNewMessagesAsync(chunks, existing, channel.ChannelId);
            await DeleteExtraMessagesAsync(existing, chunks, channel.ChannelId);
        }

        public async Task DeleteMessagesForChannelAsync(IEnumerable<TEntity> existingMessages, ulong channelId)
        {
            List<TEntity> existing = existingMessages.ToList();

            var deleteTasks = existing.Select(async message =>
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
                    return;
                }

                await _discordMessageService.DeleteMessageAsync(discordMessage.MessageId, channelId);
            });

            await Task.WhenAll(deleteTasks);
            await _messageRepository.DeleteManyAsync(existing.Select(x => x.Id));
        }

        public async Task DeleteMessageByIdAsync(TEntity message, ulong channelId)
        {
            DiscordMessageDto? discordMessage = await _discordMessageService.GetMessageAsync(
                message.DiscordMessageId,
                channelId
            );

            if (discordMessage is null)
            {
                return;
            }

            await _discordMessageService.DeleteMessageAsync(discordMessage.MessageId, channelId);
            await _messageRepository.DeleteManyAsync(new List<int> { message.Id });
        }

        public async Task CreateMessageWithComponentsAsync(
            string content,
            ulong channelId,
            List<DiscordMessageComponentDto> components
        )
        {
            DiscordMessageDto? discordMessage = await _discordMessageService.SendMessageWithComponentsAsync(
                channelId,
                content,
                components
            );

            if (discordMessage is null)
            {
                return;
            }

            await _messageRepository.CreateAsync(MessageFactory.Create(discordMessage.MessageId));
        }

        private async Task CreateMessageFromDiscordMessageBuilderAsync(
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
                return;
            }

            if (shouldSaveMessageInDatabase)
            {
                await _messageRepository.CreateAsync(MessageFactory.Create(discordMessage.MessageId));
            }
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

                    if (discordMessage is null)
                    {
                        return;
                    }

                    DiscordMessageBuilderDto messageBuilder = new DiscordMessageBuilderDto(newContent);
                    DiscordMessageDto? replacement = await _discordMessageService.SendMessageAsync(
                        channelId,
                        messageBuilder
                    );

                    if (replacement is null)
                    {
                        _logger.Warning(
                            "Failed to create a replacement message for Discord message with ID {MessageId} in channel {ChannelId}.",
                            existingMessage.DiscordMessageId,
                            channelId
                        );
                        return;
                    }

                    existingMessage.DiscordMessageId = replacement.MessageId;

                    await _discordMessageService.ModifyMessageAsync(discordMessage.MessageId, channelId, newContent);
                }
                catch
                {
                    _logger.Warning(
                        "Failed to update Discord message with ID {MessageId} in channel {ChannelId}. It may have been deleted or is inaccessible. Attempting to create a new message.",
                        existingMessage.DiscordMessageId,
                        channelId
                    );
                }
            }
        }

        private async Task CreateNewMessagesAsync(List<string> chunks, List<TEntity> existing, ulong channelId)
        {
            if (chunks.Count > existing.Count)
            {
                for (int i = existing.Count; i < chunks.Count; i++)
                {
                    DiscordMessageBuilderDto messageBuilder = new DiscordMessageBuilderDto(chunks[i]);

                    DiscordMessageDto? newMessage = await _discordMessageService.SendMessageAsync(
                        channelId,
                        messageBuilder
                    );

                    if (newMessage is null)
                    {
                        _logger.Warning("Failed to create a new message in channel {ChannelId}.", channelId);
                        continue;
                    }

                    await _messageRepository.CreateAsync(MessageFactory.Create(newMessage.MessageId));
                }
            }
        }

        private async Task DeleteExtraMessagesAsync(List<TEntity> existing, List<string> chunks, ulong channelId)
        {
            if (existing.Count > chunks.Count)
            {
                IEnumerable<TEntity> extras = existing.Skip(chunks.Count);

                await DeleteMessagesForChannelAsync(extras, channelId);
            }
        }

        private List<string> SplitMessageIntoChunks(string message)
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

                // Would this line overflow the current chunk?
                if (currentChunk.Length > 0 && currentChunk.Length + lineWithNewLine.Length > MaxChunkSize)
                {
                    chunks.Add(currentChunk.ToString().TrimEnd());
                    currentChunk.Clear();
                    currentChunk.Append(lineWithNewLine);
                    continue;
                }

                // This single line is longer than Discord allows.
                if (lineWithNewLine.Length > MaxChunkSize)
                {
                    // Flush the current chunk first.
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
