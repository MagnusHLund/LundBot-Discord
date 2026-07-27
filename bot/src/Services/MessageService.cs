using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using LundBot.Config;
using LundBot.Entities;
using LundBot.Interfaces.Repositories;
using LundBot.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace LundBot.Services
{
    public sealed class MessageService : IMessageService
    {
        private readonly DiscordConfig _discordConfig;
        private readonly IWebsiteTrafficMessagesRepository _websiteTrafficMessagesRepository;

        public MessageService(
            IOptions<DiscordConfig> options,
            IWebsiteTrafficMessagesRepository websiteTrafficMessagesRepository
        )
        {
            _discordConfig = options.Value;
            _websiteTrafficMessagesRepository = websiteTrafficMessagesRepository;
        }

        public async Task SynchronizeWebsiteTrafficMessagesAsync(
            string message,
            IEnumerable<WebsiteTrafficMessagesEntity> existingMessages,
            DiscordClient _discordClient
        )
        {
            string channelIdString = _discordConfig.WebTrafficChannelId;
            if (ulong.TryParse(channelIdString, out ulong channelId) == false)
            {
                throw new Exception("Invalid WebTrafficChannelId in configuration.");
            }

            List<string> chunks = SplitMessageIntoChunks(message);
            List<WebsiteTrafficMessagesEntity> existing = existingMessages.ToList();

            DiscordChannel channel = await _discordClient.GetChannelAsync(channelId);

            int sharedLength = Math.Min(existing.Count, chunks.Count);

            await UpdateMessagesAsync(sharedLength, existing, chunks, channel);
            await CreateNewMessagesAsync(chunks, existing, channel);
            await DeleteExtraMessagesAsync(existing, chunks, channel);
        }

        private async Task UpdateMessagesAsync(
            int sharedLength,
            List<WebsiteTrafficMessagesEntity> existing,
            List<string> chunks,
            DiscordChannel channel
        )
        {
            for (int i = 0; i < sharedLength; i++)
            {
                WebsiteTrafficMessagesEntity existingMessage = existing[i];
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

                    await _websiteTrafficMessagesRepository.UpdateAsync(existingMessage);
                }
            }
        }

        private async Task CreateNewMessagesAsync(
            List<string> chunks,
            List<WebsiteTrafficMessagesEntity> existing,
            DiscordChannel channel
        )
        {
            if (chunks.Count > existing.Count)
            {
                for (int i = existing.Count; i < chunks.Count; i++)
                {
                    DiscordMessage newMessage = await channel.SendMessageAsync(chunks[i]);

                    await _websiteTrafficMessagesRepository.CreateAsync(
                        new WebsiteTrafficMessagesEntity
                        {
                            DiscordMessageId = newMessage.Id.ToString(),
                        }
                    );
                }
            }
        }

        private async Task DeleteExtraMessagesAsync(
            List<WebsiteTrafficMessagesEntity> existing,
            List<string> chunks,
            DiscordChannel channel
        )
        {
            if (existing.Count > chunks.Count)
            {
                IEnumerable<WebsiteTrafficMessagesEntity> extras = existing.Skip(chunks.Count);

                foreach (WebsiteTrafficMessagesEntity extra in extras)
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

                await _websiteTrafficMessagesRepository.DeleteManyAsync(
                    extras.Select(x => x.WebsiteTrafficMessagesId)
                );
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
