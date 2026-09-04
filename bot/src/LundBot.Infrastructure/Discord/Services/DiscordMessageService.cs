using DSharpPlus;
using DSharpPlus.Entities;
using LundBot.Application.Common.Discord;
using Serilog;

namespace LundBot.Infrastructure.Discord.Services
{
    public sealed class DiscordMessageService : IDiscordMessageService
    {
        private readonly DiscordClient _discordClient;

        private readonly ILogger _logger = Log.ForContext<DiscordMessageService>();

        public DiscordMessageService(DiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        public async Task<DiscordMessageDto?> GetMessageAsync(ulong messageId, ulong channelId)
        {
            _logger.Information(
                "Fetching message with ID {MessageId} from channel {ChannelId}...",
                messageId,
                channelId
            );

            try
            {
                DiscordChannel channel = await _discordClient.GetChannelAsync(channelId);
                DiscordMessage message = await channel.GetMessageAsync(messageId);

                if (message is null || message.Channel is null || message.Author is null)
                {
                    return null;
                }

                return new DiscordMessageDto(
                    messageId: message.Id,
                    channelId: message.Channel.Id,
                    authorId: message.Author.Id,
                    content: message.Content
                );
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to fetch message with ID {MessageId} from channel {ChannelId}",
                    messageId,
                    channelId
                );
                return null;
            }
        }

        public async Task<DiscordMessageDto?> SendMessageAsync(ulong channelId, string content)
        {
            _logger.Information("Sending message to channel {ChannelId}...", channelId);

            try
            {
                DiscordChannel channel = await _discordClient.GetChannelAsync(channelId);
                DiscordMessage message = await channel.SendMessageAsync(content);

                return new DiscordMessageDto(
                    messageId: message.Id,
                    channelId: message.Channel!.Id,
                    authorId: message.Author!.Id,
                    content: message.Content
                );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to send message to channel {ChannelId}", channelId);
                return null;
            }
        }

        public async Task<DiscordMessageDto?> SendMessageWithComponentsAsync(
            ulong channelId,
            string content,
            IReadOnlyCollection<DiscordMessageComponentDto> components
        )
        {
            _logger.Information("Sending message with components to channel {ChannelId}...", channelId);

            try
            {
                DiscordChannel channel = await _discordClient.GetChannelAsync(channelId);
                DiscordMessageBuilder builder = new DiscordMessageBuilder().WithContent(content);

                AddComponentsToMessageBuilder(builder, components);

                DiscordMessage message = await channel.SendMessageAsync(builder);

                return new DiscordMessageDto(
                    messageId: message.Id,
                    channelId: message.Channel!.Id,
                    authorId: message.Author!.Id,
                    content: message.Content
                );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to send message with components to channel {ChannelId}", channelId);
                return null;
            }
        }

        public Task<DiscordMessageDto?> ModifyMessageAsync(ulong messageId, ulong channelId, string newContent)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteMessageAsync(ulong messageId, ulong channelId)
        {
            throw new NotImplementedException();
        }

        private static void AddComponentsToMessageBuilder(
            DiscordMessageBuilder builder,
            IReadOnlyCollection<DiscordMessageComponentDto> components
        )
        {
            foreach (DiscordMessageComponentDto component in components)
            {
                switch (component)
                {
                    case DiscordButtonDto button:
                        builder.AddActionRowComponent(
                            new DiscordButtonComponent(MapStyle(button.ButtonStyle), button.CustomId, button.Label)
                        );
                        break;

                    // TODO: This can be expanded. See AddComponentsToBuilder in the linked file for reference, https://github.com/MagnusHLund/LundBot-Discord/blob/2.4.2/bot/src/Services/Discord/DiscordMessageService.cs
                }
            }
        }

        private static DiscordButtonStyle MapStyle(DiscordButtonStyleEnum buttonStyle)
        {
            return buttonStyle switch
            {
                DiscordButtonStyleEnum.Primary => DiscordButtonStyle.Primary,
                DiscordButtonStyleEnum.Secondary => DiscordButtonStyle.Secondary,
                DiscordButtonStyleEnum.Success => DiscordButtonStyle.Success,
                DiscordButtonStyleEnum.Danger => DiscordButtonStyle.Danger,
                _ => DiscordButtonStyle.Primary,
            };
        }
    }
}
