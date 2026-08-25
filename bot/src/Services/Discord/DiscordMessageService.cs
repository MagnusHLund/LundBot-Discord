using DSharpPlus;
using DSharpPlus.Entities;
using LundBot.Interfaces.Services.Discord;
using Serilog;

namespace LundBot.Services.Discord
{
    public sealed class DiscordMessageService : IDiscordMessageService
    {
        private readonly Serilog.ILogger _logger = Log.ForContext<DiscordMessageService>();

        public async Task<DiscordMessage> GetMessageAsync(DiscordChannel channel, ulong messageId)
        {
            _logger.Information(
                "Fetching message with ID {MessageId} from channel {ChannelId}...",
                messageId,
                channel.Id
            );

            try
            {
                return await channel.GetMessageAsync(messageId);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to fetch message with ID {MessageId} from channel {ChannelId}.",
                    messageId,
                    channel.Id
                );
                throw;
            }
        }

        public async Task<DiscordMessage> SendMessageAsync(
            DiscordChannel channel,
            DiscordMessageBuilder builder
        )
        {
            _logger.Information("Sending message to channel {ChannelId}...", channel.Id);

            try
            {
                return await channel.SendMessageAsync(builder);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to send message to channel {ChannelId}.", channel.Id);
                throw;
            }
        }

        public async Task<DiscordMessage> SendMessageWithComponentsAsync(
            DiscordChannel channel,
            string content,
            List<DiscordComponent> components
        )
        {
            _logger.Information(
                "Sending message with components to channel {ChannelId}...",
                channel.Id
            );

            try
            {
                var builder = new DiscordMessageBuilder().WithContent(content);
                // .AddComponents(components.ToArray());
                // TODO: Make functionality to add any component type

                return await channel.SendMessageAsync(builder);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to send message with components to channel {ChannelId}.",
                    channel.Id
                );
                throw;
            }
        }

        public async Task<DiscordMessage> ModifyMessageAsync(DiscordMessage message, string content)
        {
            _logger.Information(
                "Modifying message with ID {MessageId} in channel {ChannelId}...",
                message.Id,
                message?.Channel?.Id
            );

            try
            {
                if (message is null)
                {
                    throw new ArgumentNullException(nameof(message), "Message cannot be null.");
                }

                return await message.ModifyAsync(content);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to modify message with ID {MessageId} in channel {ChannelId}.",
                    message?.Id,
                    message?.Channel?.Id
                );
                throw;
            }
        }

        public async Task DeleteMessageAsync(DiscordMessage message)
        {
            _logger.Information(
                "Deleting message with ID {MessageId} from channel {ChannelId}...",
                message?.Id,
                message?.Channel?.Id
            );

            try
            {
                if (message is null)
                {
                    throw new ArgumentNullException(nameof(message), "Message cannot be null.");
                }

                await message.DeleteAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to delete message with ID {MessageId} from channel {ChannelId}.",
                    message?.Id,
                    message?.Channel?.Id
                );
                throw;
            }
        }
    }
}
