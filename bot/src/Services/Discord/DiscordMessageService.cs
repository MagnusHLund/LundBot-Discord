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
                AddComponentsToBuilder(builder, components);

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

        private void AddComponentsToBuilder(
            DiscordMessageBuilder builder,
            IEnumerable<DiscordComponent> components
        )
        {
            foreach (var component in components)
            {
                switch (component)
                {
                    // V1 components
                    case DiscordButtonComponent button:
                        builder.AddActionRowComponent(button);
                        break;

                    case BaseDiscordSelectComponent select:
                        builder.AddActionRowComponent(select);
                        break;

                    // V2 components
                    case DiscordContainerComponent container:
                        builder.AddContainerComponent(container);
                        break;

                    case DiscordFileComponent file:
                        builder.AddFileComponent(file);
                        break;

                    // Is this a DSharpPlus bug? One would think that builder.AddMediaGalleryComponent would accept a DiscordMediaGalleryComponent, instead of a list of MediaGalleryItems.
                    case DiscordMediaGalleryComponent mediaGallery:
                        builder.AddMediaGalleryComponent(mediaGallery.Items);
                        break;

                    case DiscordSectionComponent section:
                        builder.AddSectionComponent(section);
                        break;

                    case DiscordSeparatorComponent separator:
                        builder.AddSeparatorComponent(separator);
                        break;

                    case DiscordTextDisplayComponent textDisplay:
                        builder.AddTextDisplayComponent(textDisplay);
                        break;

                    default:
                        _logger.Warning(
                            "Unsupported component type: {ComponentType}. Component will not be added.",
                            component.GetType().Name
                        );
                        break;
                }
            }
        }
    }
}
