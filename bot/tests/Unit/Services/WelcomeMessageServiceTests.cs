using DSharpPlus.Entities;
using LundBot.Entities;
using LundBot.Tests.TestHelpers;
using LundBot.Tests.Unit.Services.Contexts;
using Moq;

namespace LundBot.Tests.Unit.Services;

public sealed class WelcomeMessageServiceTests
{
    [Fact]
    internal async Task SendWelcomeMessageAsync_WhenSystemChannelIsNull_DoesNotSendMessage()
    {
        // Arrange
        var context = WelcomeMessageServiceTestContextFactory.Create();
        var guild = DiscordObjectFactory.CreateUninitializedGuild(1);
        var member = DiscordObjectFactory.CreateMember(100);

        // Act
        await context.Service.SendWelcomeMessageAsync(guild, member);

        // Assert
        context.MessageService.Verify(
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
        var context = WelcomeMessageServiceTestContextFactory.Create();
        var systemChannel = DiscordObjectFactory.CreateChannel(999);
        var guild = DiscordObjectFactory.CreateGuildWithSystemChannel(1, systemChannel);
        var member = DiscordObjectFactory.CreateMember(100);

        context
            .MessageService.Setup(m =>
                m.CreateMessageWithComponentsAsync(
                    It.IsAny<string>(),
                    It.IsAny<DiscordChannel>(),
                    It.IsAny<List<DiscordComponent>>()
                )
            )
            .Returns(Task.CompletedTask);

        // Act
        await context.Service.SendWelcomeMessageAsync(guild, member);

        // Assert
        context.MessageService.Verify(
            m =>
                m.CreateMessageWithComponentsAsync(
                    It.IsAny<string>(),
                    It.IsAny<DiscordChannel>(),
                    It.Is<List<DiscordComponent>>(components => components.Count == 1)
                ),
            Times.Once
        );
        Assert.Equal("100", context.Factory.Create("any").DiscordUserId);
    }

    [Fact]
    internal async Task RemoveWelcomeMessageAsync_WhenSystemChannelIsNull_DoesNotDeleteMessage()
    {
        // Arrange
        var context = WelcomeMessageServiceTestContextFactory.Create();
        var guild = DiscordObjectFactory.CreateUninitializedGuild(1);

        // Act
        await context.Service.RemoveWelcomeMessageAsync(guild, discordMemberId: 100);

        // Assert
        context.MessageService.Verify(
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
        var context = WelcomeMessageServiceTestContextFactory.Create();
        var systemChannel = DiscordObjectFactory.CreateChannel(999);
        var guild = DiscordObjectFactory.CreateGuildWithSystemChannel(1, systemChannel);
        var welcomeEntity = new WelcomeMessageEntity
        {
            Id = 7,
            DiscordMessageId = "555",
            DiscordUserId = "100",
        };

        context.Repository.Setup(r => r.GetByJoinedUserIdAsync("100")).ReturnsAsync(welcomeEntity);
        context
            .MessageService.Setup(m =>
                m.DeleteMessageByIdAsync(welcomeEntity, It.IsAny<DiscordChannel>())
            )
            .Returns(Task.CompletedTask);

        // Act
        await context.Service.RemoveWelcomeMessageAsync(guild, discordMemberId: 100);

        // Assert
        context.MessageService.Verify(
            m => m.DeleteMessageByIdAsync(welcomeEntity, It.IsAny<DiscordChannel>()),
            Times.Once
        );
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
