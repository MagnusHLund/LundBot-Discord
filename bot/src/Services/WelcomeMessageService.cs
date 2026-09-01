using DSharpPlus.Entities;
using LundBot.Entities;
using LundBot.Factories.MessageEntityFactories;
using LundBot.Interfaces.Repositories;
using LundBot.Interfaces.Services;
using LundBot.Interfaces.Services.Discord;
using LundBot.Models;
using LundBot.Repositories;

namespace LundBot.Services
{
    public sealed class WelcomeMessageService : IWelcomeMessageService
    {
        private readonly IDiscordChannelService _discordChannelService;
        private readonly IDiscordStickerService _discordStickerService;
        private readonly IMessageService<
            WelcomeMessageEntity,
            WelcomeMessagesRepository,
            WelcomeMessageFactory
        > _messageService;
        private readonly IWelcomeMessagesRepository _welcomeMessagesRepository;
        private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<WelcomeMessageService>();

        public WelcomeMessageService(
            IMessageService<
                WelcomeMessageEntity,
                WelcomeMessagesRepository,
                WelcomeMessageFactory
            > messageService,
            IWelcomeMessagesRepository welcomeMessagesRepository,
            IDiscordStickerService discordStickerService,
            IDiscordChannelService discordChannelService
        )
        {
            _messageService = messageService;
            _welcomeMessagesRepository = welcomeMessagesRepository;
            _discordStickerService = discordStickerService;
            _discordChannelService = discordChannelService;
        }

        public async Task SendWelcomeMessageAsync(DiscordGuild guild, DiscordMember member)
        {
            _logger.Information(
                "Sending welcome message for user {UserId} in guild {GuildId}",
                member.Id,
                guild.Id
            );

            DiscordChannel? systemChannel = await _discordChannelService.GetSystemChannelAsync(
                guild
            );

            if (systemChannel is null)
            {
                _logger.Warning("No system channel found for guild {GuildId}", guild.Id);
                return;
            }

            short randomIndex = (short)new Random().Next(WelcomeMessages.Messages.Count);
            string welcomeMessage = string.Format(
                WelcomeMessages.Messages[randomIndex],
                member.Mention
            );

            _messageService.MessageFactory.SetJoinedUserId(member.Id.ToString());

            string interactionId = $"welcome_hi:{member.Id}";
            await _messageService.CreateMessageWithComponentsAsync(
                welcomeMessage,
                systemChannel,
                new List<DiscordComponent>
                {
                    new DiscordButtonComponent(
                        DiscordButtonStyle.Primary,
                        interactionId,
                        "Say Hi 👋"
                    ),
                }
            );

            _logger.Information(
                "Welcome message sent for user {UserId} in guild {GuildId}",
                member.Id,
                guild.Id
            );
        }

        public async Task HandleWelcomeInteractionAsync(
            DiscordUser senderUser,
            DiscordMember targetUser,
            DiscordChannel channel
        )
        {
            var message = new DiscordMessageBuilder().WithContent(
                $"{senderUser.Mention} says hi to {targetUser.Mention}"
            );

            var welcomeStickers = await GetWelcomeStickersAsync();
            if (welcomeStickers.Count > 0)
            {
                var randomSticker = welcomeStickers[Random.Shared.Next(welcomeStickers.Count)];
                message.WithStickers(new List<DiscordMessageSticker> { randomSticker });
            }

            var welcomeMessage = await _welcomeMessagesRepository.GetByJoinedUserIdAsync(
                targetUser.Id.ToString()
            );

            ulong replyMessageId = ulong.Parse(welcomeMessage.DiscordMessageId);
            message.WithReply(replyMessageId);

            await _messageService.CreateMessageFromDiscordMessageBuilderAsync(message, channel);
        }

        public async Task RemoveWelcomeMessageAsync(DiscordGuild guild, ulong discordMemberId)
        {
            _logger.Information(
                "Removing welcome message for user {UserId} in guild {GuildId}",
                discordMemberId,
                guild.Id
            );

            DiscordChannel? systemChannel = await _discordChannelService.GetSystemChannelAsync(
                guild
            );

            if (systemChannel is null)
            {
                _logger.Warning("No system channel found for guild {GuildId}", guild.Id);
                return;
            }

            WelcomeMessageEntity welcomeMessage;
            try
            {
                welcomeMessage = await _welcomeMessagesRepository.GetByJoinedUserIdAsync(
                    discordMemberId.ToString()
                );
            }
            catch (KeyNotFoundException)
            {
                _logger.Warning(
                    "No welcome message found for user {UserId} in guild {GuildId}; nothing to remove.",
                    discordMemberId,
                    guild.Id
                );
                return;
            }

            await _messageService.DeleteMessageByIdAsync(welcomeMessage, systemChannel);

            _logger.Information(
                "Welcome message removed for user {UserId} in guild {GuildId}",
                discordMemberId,
                guild.Id
            );
        }

        private async Task<List<DiscordMessageSticker>> GetWelcomeStickersAsync()
        {
            List<string> uniqueTitles = new List<string>() { "Wave", "Heya", "Sup", "Hello" };

            var stickerPacks = await _discordStickerService.GetStickerPacksAsync();

            List<DiscordMessageSticker> welcomeStickers = new List<DiscordMessageSticker>();
            foreach (var pack in stickerPacks)
            {
                foreach (var sticker in pack.Stickers)
                {
                    if (sticker.Name is not null && uniqueTitles.Contains(sticker.Name))
                    {
                        welcomeStickers.Add(sticker);
                    }
                }
            }

            return welcomeStickers;
        }
    }
}
