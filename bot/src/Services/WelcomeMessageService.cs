using DSharpPlus.Entities;
using LundBot.Entities;
using LundBot.Factories.MessageEntityFactories;
using LundBot.Interfaces.Repositories;
using LundBot.Interfaces.Services;
using LundBot.Repositories;

namespace LundBot.Services
{
    public sealed class WelcomeMessageService : IWelcomeMessageService
    {
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
            IWelcomeMessagesRepository welcomeMessagesRepository
        )
        {
            _messageService = messageService;
            _welcomeMessagesRepository = welcomeMessagesRepository;
        }

        public async Task SendWelcomeMessageAsync(DiscordGuild guild, DiscordMember member)
        {
            _logger.Information(
                "Sending welcome message for user {UserId} in guild {GuildId}",
                member.Id,
                guild.Id
            );

            DiscordChannel systemChannel = guild.SystemChannel;

            if (systemChannel is null)
            {
                _logger.Warning("No system channel found for guild {GuildId}", guild.Id);
                return;
            }

            // TODO: Get random IW related welcome message from some dictionary somewhere.

            _messageService.MessageFactory.SetJoinedUserId(member.Id.ToString());

            await _messageService.SynchronizeDiscordMessagesAsync(
                $"Welcome {member.Mention}!",
                Enumerable.Empty<WelcomeMessageEntity>(),
                systemChannel.Id
            );

            _logger.Information(
                "Welcome message sent for user {UserId} in guild {GuildId}",
                member.Id,
                guild.Id
            );
        }

        // TODO: Use method
        public async Task RemoveWelcomeMessageAsync(DiscordGuild guild, ulong discordMemberId)
        {
            _logger.Information(
                "Removing welcome message for user {UserId} in guild {GuildId}",
                discordMemberId,
                guild.Id
            );

            DiscordChannel systemChannel = guild.SystemChannel;

            if (systemChannel is null)
            {
                _logger.Warning("No system channel found for guild {GuildId}", guild.Id);
                return;
            }

            WelcomeMessageEntity welcomeMessage =
                await _welcomeMessagesRepository.GetByJoinedUserIdAsync(discordMemberId.ToString());

            await _messageService.DeleteMessageByIdAsync(welcomeMessage, systemChannel);

            _logger.Information(
                "Welcome message removed for user {UserId} in guild {GuildId}",
                discordMemberId,
                guild.Id
            );
        }
    }
}
