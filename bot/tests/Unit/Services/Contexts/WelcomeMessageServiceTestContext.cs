using LundBot.Entities;
using LundBot.Factories.MessageEntityFactories;
using LundBot.Interfaces.Repositories;
using LundBot.Interfaces.Services;
using LundBot.Interfaces.Services.Discord;
using LundBot.Repositories;
using LundBot.Services;
using Moq;

namespace LundBot.Tests.Unit.Services.Contexts
{
    internal sealed record WelcomeMessageServiceTestContext(
        WelcomeMessageService Service,
        Mock<
            IMessageService<WelcomeMessageEntity, WelcomeMessagesRepository, WelcomeMessageFactory>
        > MessageService,
        WelcomeMessageFactory Factory,
        Mock<IWelcomeMessagesRepository> Repository,
        Mock<IDiscordStickerService> StickerService,
        Mock<IDiscordChannelService> DiscordChannelService
    );
}
