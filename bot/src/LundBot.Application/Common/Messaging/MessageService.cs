using System.Text;
using LundBot.Application.Discord.Channels;
using LundBot.Application.Discord.Interactions;
using LundBot.Application.Discord.Messages;
using LundBot.Domain.Common;

namespace LundBot.Application.Common.Messaging
{
    public sealed class MessageService<TEntity, TRepository, TFactory>
        where TRepository : IMessageRepository<TEntity>
        where TEntity : AbstractMessageEntity, new()
        where TFactory : IMessageEntityFactory<TEntity>, IMessageService<TEntity, TFactory>
    {
        private readonly TRepository _messageRepository;

        // TODO: Why was this a property in the old project?
        private TFactory MessageFactory { get; set; }

        public MessageService(TRepository messageRepository, TFactory messageFactory)
        {
            _messageRepository = messageRepository;
            MessageFactory = messageFactory;
        }

        public async Task SynchronizeDiscordMessagesAsync(
            string message,
            IEnumerable<TEntity> existingMessages,
            ulong channelId
        )
        {
            List<string> chunks = SplitMessageIntoChunks(message);
            List<TEntity> existing = existingMessages.ToList();

            DiscordChannel channel = await _discordChannelService.GetChannelAsync(channelId);

            int sharedLength = Math.Min(existing.Count, chunks.Count);

            await UpdateMessagesAsync(sharedLength, existing, chunks, channel);
            await CreateNewMessagesAsync(chunks, existing, channel);
            await DeleteExtraMessagesAsync(existing, chunks, channel);
        }

        Task DeleteMessagesForChannelAsync(IEnumerable<TEntity> existingMessages, DiscordChannelDto channel)
        {
            List<TEntity> existing = existingMessages.ToList();

            var deleteTasks = existing.Select(async message =>
            {
                var discordMessage = await _discordMessageService.GetMessageAsync(
                    channel,
                    ulong.Parse(message.DiscordMessageId)
                );

                await _discordMessageService.DeleteMessageAsync(discordMessage);
            });

            await Task.WhenAll(deleteTasks);
            await _messageRepository.DeleteManyAsync(existing.Select(x => x.Id));
        }

        public async Task DeleteMessageByIdAsync(TEntity message, DiscordChannelDto channel)
        {
            var discordMessage = await _discordMessageService.GetMessageAsync(
                channel,
                ulong.Parse(message.DiscordMessageId)
            );

            await _discordMessageService.DeleteMessageAsync(discordMessage);
            await _messageRepository.DeleteManyAsync(new List<int> { message.Id });
        }

        Task DeleteMessageByIdAsync(TEntity message, DiscordChannelDto channel)
        {
            var discordMessage = await _discordMessageService.GetMessageAsync(
                channel,
                ulong.Parse(message.DiscordMessageId)
            );

            await _discordMessageService.DeleteMessageAsync(discordMessage);
            await _messageRepository.DeleteManyAsync(new List<int> { message.Id });
        }

        Task CreateMessageWithComponentsAsync(
            string content,
            DiscordChannelDto channel,
            List<DiscordMessageComponentDto> components
        )
        {
            DiscordMessage discordMessage = await _discordMessageService.SendMessageWithComponentsAsync(
                channel,
                content,
                components
            );

            await _messageRepository.CreateAsync(_messageFactory.Create(discordMessage.Id.ToString()));
        }

        Task CreateMessageFromDiscordMessageBuilderAsync(
            DiscordMessageBuilderDto messageBuilder,
            DiscordChannelDto channel,
            bool shouldSaveMessageInDatabase = false
        )
        {
            DiscordMessage discordMessage = await _discordMessageService.SendMessageAsync(channel, messageBuilder);

            if (shouldSaveMessage)
            {
                await _messageRepository.CreateAsync(_messageFactory.Create(discordMessage.Id.ToString()));
            }
        }

        private async Task UpdateMessagesAsync(
            int sharedLength,
            List<TEntity> existing,
            List<string> chunks,
            DiscordChannel channel
        )
        {
            for (int i = 0; i < sharedLength; i++)
            {
                TEntity existingMessage = existing[i];
                string newContent = chunks[i];

                try
                {
                    DiscordMessage discordMessage = await _discordMessageService.GetMessageAsync(
                        channel,
                        ulong.Parse(existingMessage.DiscordMessageId)
                    );

                    await _discordMessageService.ModifyMessageAsync(discordMessage, newContent);
                }
                catch
                {
                    _logger.Warning(
                        "Failed to update Discord message with ID {MessageId} in channel {ChannelId}. It may have been deleted or is inaccessible. Attempting to create a new message.",
                        existingMessage.DiscordMessageId,
                        channel.Id
                    );

                    var messageBuilder = new DiscordMessageBuilder().WithContent(newContent);

                    DiscordMessage replacement = await _discordMessageService.SendMessageAsync(channel, messageBuilder);

                    existingMessage.DiscordMessageId = replacement.Id.ToString();

                    await _messageRepository.UpdateAsync(existingMessage);
                }
            }
        }

        private async Task CreateNewMessagesAsync(List<string> chunks, List<TEntity> existing, DiscordChannelDto channel)
        {
            if (chunks.Count > existing.Count)
            {
                for (int i = existing.Count; i < chunks.Count; i++)
                {
                    var messageBuilder = new DiscordMessageBuilder().WithContent(chunks[i]);

                    DiscordMessage newMessage = await _discordMessageService.SendMessageAsync(channel, messageBuilder);

                    await _messageRepository.CreateAsync(_messageFactory.Create(newMessage.Id.ToString()));
                }
            }
        }

        private async Task DeleteExtraMessagesAsync(List<TEntity> existing, List<string> chunks, DiscordChannelDto channel)
        {
            if (existing.Count > chunks.Count)
            {
                IEnumerable<TEntity> extras = existing.Skip(chunks.Count);

                await DeleteMessagesForChannelAsync(extras, channel);
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
