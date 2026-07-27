using System.Text;
using DSharpPlus.Entities;
using LundBot.Entities;
using LundBot.Factories.MessageEntityFactories;
using LundBot.Interfaces.Services;
using LundBot.Repositories;

namespace LundBot.Services
{
    public sealed class MessageService<TEntity, TRepository, TFactory>
        : IMessageService<TEntity, TRepository, TFactory>
        where TRepository : AbstractMessageRepository<TEntity>
        where TEntity : AbstractMessageEntity, new()
        where TFactory : IMessageEntityFactory<TEntity>
    {
        public TFactory MessageFactory { get; }
        private readonly TRepository _messageRepository;
        private readonly TFactory _messageFactory;

        public MessageService(TRepository messageRepository, TFactory messageFactory)
        {
            _messageRepository = messageRepository;
            _messageFactory = messageFactory;
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

            DiscordChannel channel = await BotService.DiscordClient.GetChannelAsync(channelId);

            int sharedLength = Math.Min(existing.Count, chunks.Count);

            await UpdateMessagesAsync(sharedLength, existing, chunks, channel);
            await CreateNewMessagesAsync(chunks, existing, channel);
            await DeleteExtraMessagesAsync(existing, chunks, channel);
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
                    DiscordMessage discordMessage = await channel.GetMessageAsync(
                        ulong.Parse(existingMessage.DiscordMessageId)
                    );

                    await discordMessage.ModifyAsync(newContent);
                }
                catch
                {
                    // Message no longer exists. Create a replacement.
                    DiscordMessage replacement = await channel.SendMessageAsync(newContent);

                    existingMessage.DiscordMessageId = replacement.Id.ToString();

                    await _messageRepository.UpdateAsync(existingMessage);
                }
            }
        }

        private async Task CreateNewMessagesAsync(
            List<string> chunks,
            List<TEntity> existing,
            DiscordChannel channel
        )
        {
            if (chunks.Count > existing.Count)
            {
                for (int i = existing.Count; i < chunks.Count; i++)
                {
                    DiscordMessage newMessage = await channel.SendMessageAsync(chunks[i]);

                    await _messageRepository.CreateAsync(
                        _messageFactory.Create(newMessage.Id.ToString())
                    );
                }
            }
        }

        private async Task DeleteExtraMessagesAsync(
            List<TEntity> existing,
            List<string> chunks,
            DiscordChannel channel
        )
        {
            if (existing.Count > chunks.Count)
            {
                IEnumerable<TEntity> extras = existing.Skip(chunks.Count);

                foreach (TEntity extra in extras)
                {
                    try
                    {
                        DiscordMessage discordMessage = await channel.GetMessageAsync(
                            ulong.Parse(extra.DiscordMessageId)
                        );

                        await discordMessage.DeleteAsync();
                    }
                    catch
                    {
                        // Ignore if it was already deleted.
                    }
                }

                await _messageRepository.DeleteManyAsync(extras.Select(x => x.Id));
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
                if (
                    currentChunk.Length > 0
                    && currentChunk.Length + lineWithNewLine.Length > MaxChunkSize
                )
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
