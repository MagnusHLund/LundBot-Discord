using LundBot.Application.Common.Messaging;
using LundBot.Application.Discord.Channels;
using LundBot.Application.Discord.Interactions;
using LundBot.Application.Discord.Members;
using LundBot.Application.Discord.Messages;
using LundBot.Application.Discord.Stickers;
using LundBot.Domain.MemberJoin;

namespace LundBot.Application.Features.MemberJoin
{
    public sealed class MemberJoinService : IMemberJoinService
    {
        private readonly IDiscordMemberService _discordMemberService;
        private readonly IDiscordStickerService _discordStickerService;
        private readonly IMemberJoinMessageRepository _memberJoinMessageRepository;
        private readonly IDiscordChannelService _discordChannelService;
        private readonly IMessageService<
            MemberJoinMessage,
            IMemberJoinMessageRepository,
            MemberJoinMessageFactory
        > _messageService;

        private readonly ILogger _logger = Log.ForContext<MemberJoinService>();

        public MemberJoinService(
            IDiscordMemberService discordMemberService,
            IMemberJoinMessageRepository memberJoinMessageRepository,
            IDiscordChannelService discordChannelService,
            IDiscordStickerService discordStickerService,
            IMessageService<MemberJoinMessage, IMemberJoinMessageRepository, MemberJoinMessageFactory> messageService
        )
        {
            _discordMemberService = discordMemberService;
            _memberJoinMessageRepository = memberJoinMessageRepository;
            _discordChannelService = discordChannelService;
            _discordStickerService = discordStickerService;
            _messageService = messageService;
        }

        public async Task SendWelcomeMessageAsync(ulong guildId, DiscordMemberDto member)
        {
            _logger.Information("Sending welcome message for user {UserId} in guild {GuildId}", member.UserId, guildId);

            DiscordChannelDto? systemChannel = await _discordChannelService.GetSystemChannelAsync(guildId);

            if (systemChannel is null)
            {
                _logger.Warning("No system channel found for guild {GuildId}", guildId);
                return;
            }

            short randomIndex = (short)Random.Shared.Next(WelcomeMessages.Messages.Count);
            string welcomeMessage = string.Format(WelcomeMessages.Messages[randomIndex], $"**{member.DisplayName}**");

            _messageService.MessageFactory.SetJoinedUserId(member.UserId);

            string content = "Say Hi 👋";
            string interactionId = $"memberJoin_hi:{member.UserId}";
            await _messageService.CreateMessageWithComponentsAsync(
                welcomeMessage,
                systemChannel,
                new List<DiscordMessageComponentDto>
                {
                    new DiscordButtonDto(interactionId, content, DiscordButtonStyleEnum.Primary),
                }
            );

            _logger.Information("Welcome message sent for user {UserId} in guild {GuildId}", member.UserId, guildId);
        }

        public async Task<bool> HandleMemberJoinHiEventAsync(
            ulong senderUserId,
            ulong targetUserId,
            ulong systemChannelId,
            ulong guildId
        )
        {
            DiscordMemberDto? senderUser = await _discordMemberService.GetMemberAsync(senderUserId, guildId);
            DiscordMemberDto? targetUser = await _discordMemberService.GetMemberAsync(targetUserId, guildId);

            if (senderUser is null || targetUser is null)
            {
                return false;
            }

            DiscordMessageBuilderDto message = new DiscordMessageBuilderDto(
                content: $"**{senderUser.Username}** says hi to **{targetUser.Username}**"
            );

            var welcomeStickers = await GetWelcomeStickersAsync();
            if (welcomeStickers.Count > 0)
            {
                var randomSticker = welcomeStickers[Random.Shared.Next(welcomeStickers.Count)];
                message.StickerIds = new List<ulong> { randomSticker.StickerId };
            }

            MemberJoinMessage? welcomeMessage = await _memberJoinMessageRepository.GetByJoinedUserIdAsync(
                targetUser.UserId
            );

            if (welcomeMessage is null)
            {
                return false;
            }

            message.ReplyToMessageId = welcomeMessage.DiscordMessageId;
            DiscordMessageDto? createdMessage = await _messageService.CreateMessageFromDiscordMessageBuilderAsync(
                message,
                systemChannelId
            );

            return createdMessage is not null;
        }

        public async Task RemoveWelcomeMessageAsync(ulong guildId, ulong discordMemberId)
        {
            _logger.Information(
                "Removing welcome message for user {UserId} in guild {GuildId}",
                discordMemberId,
                guildId
            );

            DiscordChannelDto? systemChannel = await _discordChannelService.GetSystemChannelAsync(guildId);

            if (systemChannel is null)
            {
                _logger.Warning("No system channel found for guild {GuildId}", guildId);
                return;
            }

            MemberJoinMessage? welcomeMessage;
            try
            {
                welcomeMessage = await _memberJoinMessageRepository.GetByJoinedUserIdAsync(discordMemberId);
            }
            catch (KeyNotFoundException)
            {
                _logger.Warning(
                    "No welcome message found for user {UserId} in guild {GuildId}; nothing to remove.",
                    discordMemberId,
                    guildId
                );
                return;
            }

            if (welcomeMessage is null)
            {
                _logger.Warning(
                    "No welcome message found for user {UserId} in guild {GuildId}; nothing to remove.",
                    discordMemberId,
                    guildId
                );
                return;
            }

            await _messageService.DeleteMessageByIdAsync(welcomeMessage, systemChannel);

            _logger.Information(
                "Welcome message removed for user {UserId} in guild {GuildId}",
                discordMemberId,
                guildId
            );
        }

        private async Task<List<DiscordStickerDto>> GetWelcomeStickersAsync()
        {
            List<string> uniqueTitles = new List<string>() { "Wave", "Heya", "Sup", "Hello" };

            var stickerPacks = await _discordStickerService.GetAllStickerPacksAsync();

            List<DiscordStickerDto> welcomeStickers = new List<DiscordStickerDto>();
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
