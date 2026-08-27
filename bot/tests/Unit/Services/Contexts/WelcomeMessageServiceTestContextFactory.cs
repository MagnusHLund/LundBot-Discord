using LundBot.Entities;
using LundBot.Factories.MessageEntityFactories;
using LundBot.Interfaces.Repositories;
using LundBot.Interfaces.Services;
using LundBot.Interfaces.Services.Discord;
using LundBot.Repositories;
using LundBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace LundBot.Tests.Unit.Services.Contexts
{
    internal static class WelcomeMessageServiceTestContextFactory
    {
        internal static WelcomeMessageServiceTestContext Create()
        {
            var mockMessageService =
                new Mock<
                    IMessageService<
                        WelcomeMessageEntity,
                        WelcomeMessagesRepository,
                        WelcomeMessageFactory
                    >
                >();

            var scope = new Mock<IServiceScopeFactory>();

            var factory = new WelcomeMessageFactory();
            mockMessageService.SetupGet(m => m.MessageFactory).Returns(factory);

            var mockRepository = new Mock<IWelcomeMessagesRepository>();
            var mockStickerService = new Mock<IDiscordStickerService>();
            var mockDiscordChannelService = new Mock<IDiscordChannelService>();

            var service = new WelcomeMessageService(
                mockMessageService.Object,
                scope.Object,
                mockRepository.Object,
                mockStickerService.Object,
                mockDiscordChannelService.Object
            );

            return new WelcomeMessageServiceTestContext(
                service,
                mockMessageService,
                factory,
                mockRepository,
                mockStickerService,
                mockDiscordChannelService
            );
        }
    }
}
