using DSharpPlus.Entities;
using LundBot.Entities;
using LundBot.Factories.MessageEntityFactories;
using LundBot.Interfaces.Repositories;
using LundBot.Interfaces.Services;
using LundBot.Interfaces.Services.Discord;
using LundBot.Repositories;
using LundBot.Services;
using LundBot.Tests.TestHelpers;
using Moq;

namespace LundBot.Tests.Unit.Services;

public sealed class WelcomeMessageServiceTests
{
    private static (
        WelcomeMessageService Service,
        Mock<
            IMessageService<WelcomeMessageEntity, WelcomeMessagesRepository, WelcomeMessageFactory>
        > MessageService,
        WelcomeMessageFactory Factory,
        Mock<IWelcomeMessagesRepository> Repository,
        Mock<IDiscordStickerService> StickerService,
        Mock<IDiscordChannelService> DiscordChannelService
    ) CreateContext()
    {
        var mockMessageService =
            new Mock<
                IMessageService<
                    WelcomeMessageEntity,
                    WelcomeMessagesRepository,
                    WelcomeMessageFactory
                >
            >();
        var factory = new WelcomeMessageFactory();
        mockMessageService.SetupGet(m => m.MessageFactory).Returns(factory);

        var mockRepository = new Mock<IWelcomeMessagesRepository>();
        var mockStickerService = new Mock<IDiscordStickerService>();
        var mockDiscordChannelService = new Mock<IDiscordChannelService>();

        var service = new WelcomeMessageService(
            mockMessageService.Object,
            mockRepository.Object,
            mockStickerService.Object,
            mockDiscordChannelService.Object
        );

        return (
            service,
            mockMessageService,
            factory,
            mockRepository,
            mockStickerService,
            mockDiscordChannelService
        );
    }

    [Fact]
    internal async Task SendWelcomeMessageAsync_WhenSystemChannelIsNull_DoesNotSendMessage()
    {
        // Arrange
        var (service, mockMessageService, _, _, _, _) = CreateContext();
        var guild = DiscordObjectFactory.CreateUninitializedGuild(1);
        var member = DiscordObjectFactory.CreateMember(100);

        // Act
        await service.SendWelcomeMessageAsync(guild, member);

        // Assert
        mockMessageService.Verify(
            m =>
                m.CreateMessageWithComponentsAsync(
                    It.IsAny<string>(),
                    It.IsAny<DiscordChannel>(),
                    It.IsAny<List<DiscordComponent>>()
                ),
            Times.Never
        );
    }

    [Fact]
    internal async Task SendWelcomeMessageAsync_WhenSystemChannelExists_SendsMessageWithButton()
    {
        // Arrange
        var (service, mockMessageService, factory, _, _, _) = CreateContext();
        var systemChannel = DiscordObjectFactory.CreateChannel(999);
        var guild = DiscordObjectFactory.CreateGuildWithSystemChannel(1, systemChannel);
        var member = DiscordObjectFactory.CreateMember(100);

        mockMessageService
            .Setup(m =>
                m.CreateMessageWithComponentsAsync(
                    It.IsAny<string>(),
                    It.IsAny<DiscordChannel>(),
                    It.IsAny<List<DiscordComponent>>()
                )
            )
            .Returns(Task.CompletedTask);

        // Act
        await service.SendWelcomeMessageAsync(guild, member);

        // Assert
        mockMessageService.Verify(
            m =>
                m.CreateMessageWithComponentsAsync(
                    It.IsAny<string>(),
                    It.IsAny<DiscordChannel>(),
                    It.Is<List<DiscordComponent>>(components => components.Count == 1)
                ),
            Times.Once
        );
        Assert.Equal("100", factory.Create("any").DiscordUserId);
    }

    [Fact]
    internal async Task RemoveWelcomeMessageAsync_WhenSystemChannelIsNull_DoesNotDeleteMessage()
    {
        // Arrange
        var (service, mockMessageService, _, _, _, _) = CreateContext();
        var guild = DiscordObjectFactory.CreateUninitializedGuild(1);

        // Act
        await service.RemoveWelcomeMessageAsync(guild, discordMemberId: 100);

        // Assert
        mockMessageService.Verify(
            m =>
                m.DeleteMessageByIdAsync(
                    It.IsAny<WelcomeMessageEntity>(),
                    It.IsAny<DiscordChannel>()
                ),
            Times.Never
        );
    }

    [Fact]
    internal async Task RemoveWelcomeMessageAsync_WhenWelcomeMessageExists_DeletesMessageAndRemovesFromRepository()
    {
        // Arrange
        var (service, mockMessageService, _, mockRepository, _, _) = CreateContext();
        var systemChannel = DiscordObjectFactory.CreateChannel(999);
        var guild = DiscordObjectFactory.CreateGuildWithSystemChannel(1, systemChannel);
        var welcomeEntity = new WelcomeMessageEntity
        {
            Id = 7,
            DiscordMessageId = "555",
            DiscordUserId = "100",
        };

        mockRepository.Setup(r => r.GetByJoinedUserIdAsync("100")).ReturnsAsync(welcomeEntity);
        mockMessageService
            .Setup(m => m.DeleteMessageByIdAsync(welcomeEntity, It.IsAny<DiscordChannel>()))
            .Returns(Task.CompletedTask);

        // Act
        await service.RemoveWelcomeMessageAsync(guild, discordMemberId: 100);

        // Assert
        mockMessageService.Verify(
            m => m.DeleteMessageByIdAsync(welcomeEntity, It.IsAny<DiscordChannel>()),
            Times.Once
        );
    }

    [Fact]
    internal async Task GetWelcomeStickersAsync_ReturnsOnlyStickersMatchingWelcomeTitles()
    {
        // Arrange
        var (service, _, _, _, mockStickerService, _) = CreateContext();

        var waveSticker = CreateSticker(1, "Wave");
        var heyaSticker = CreateSticker(2, "Heya");
        var supSticker = CreateSticker(3, "Sup");
        var helloSticker = CreateSticker(4, "Hello");
        var otherSticker = CreateSticker(5, "SomeOtherSticker");

        var pack = CreateStickerPack(
            new Dictionary<ulong, DiscordMessageSticker>
            {
                [1] = waveSticker,
                [2] = heyaSticker,
                [3] = supSticker,
                [4] = helloSticker,
                [5] = otherSticker,
            }
        );

        mockStickerService
            .Setup(s => s.GetStickerPacksAsync())
            .ReturnsAsync(new List<DiscordMessageStickerPack> { pack });

        // Act
        List<DiscordMessageSticker> result = await service.GetWelcomeStickersAsync();

        // Assert
        Assert.Equal(4, result.Count);
        Assert.DoesNotContain(result, s => s.Name == "SomeOtherSticker");
        Assert.Contains(result, s => s.Name == "Wave");
        Assert.Contains(result, s => s.Name == "Heya");
        Assert.Contains(result, s => s.Name == "Sup");
        Assert.Contains(result, s => s.Name == "Hello");
    }

    [Fact]
    internal async Task GetWelcomeStickersAsync_WhenNoMatchingStickers_ReturnsEmptyList()
    {
        // Arrange
        var (service, _, _, _, mockStickerService, _) = CreateContext();

        var pack = CreateStickerPack(
            new Dictionary<ulong, DiscordMessageSticker>
            {
                [1] = CreateSticker(1, "Angry"),
                [2] = CreateSticker(2, "Sad"),
            }
        );

        mockStickerService
            .Setup(s => s.GetStickerPacksAsync())
            .ReturnsAsync(new List<DiscordMessageStickerPack> { pack });

        // Act
        List<DiscordMessageSticker> result = await service.GetWelcomeStickersAsync();

        // Assert
        Assert.Empty(result);
    }

    private static DiscordMessageSticker CreateSticker(ulong id, string name)
    {
        var sticker = (DiscordMessageSticker)
            System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(
                typeof(DiscordMessageSticker)
            );
        SetField(sticker, "Id", id);
        SetField(sticker, "Name", name);
        return sticker;
    }

    private static DiscordMessageStickerPack CreateStickerPack(
        Dictionary<ulong, DiscordMessageSticker> stickers
    )
    {
        var pack = (DiscordMessageStickerPack)
            System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(
                typeof(DiscordMessageStickerPack)
            );
        // DSharpPlus stores stickers in a private Dictionary<ulong, DiscordMessageSticker> _stickers field
        SetField(pack, "_stickers", stickers);
        return pack;
    }

    private static void SetField(object target, string memberName, object value)
    {
        var type = target.GetType();
        var property = type.GetProperty(
            memberName,
            System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.NonPublic
        );
        if (property?.SetMethod is not null)
        {
            property.SetValue(target, value);
            return;
        }

        System.Reflection.FieldInfo? field = null;
        var current = type;
        while (current is not null && field is null)
        {
            field =
                current.GetField(
                    $"<{memberName}>k__BackingField",
                    System.Reflection.BindingFlags.Instance
                        | System.Reflection.BindingFlags.NonPublic
                )
                ?? current.GetField(
                    memberName,
                    System.Reflection.BindingFlags.Instance
                        | System.Reflection.BindingFlags.Public
                        | System.Reflection.BindingFlags.NonPublic
                );
            current = current.BaseType;
        }

        if (field is null)
            throw new InvalidOperationException($"Cannot set '{memberName}' on '{type.FullName}'.");

        field.SetValue(target, value);
    }
}
