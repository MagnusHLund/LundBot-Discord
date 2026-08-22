using LundBot.Services;
using LundBot.Tests.TestHelpers;
using Moq;
using LundBot.Interfaces.Services.Discord;

namespace LundBot.Tests.Unit.Services;

public sealed class ModerationActionsServiceTests
{
    [Fact]
    internal async Task KickUserDueToRoleAssignmentAsync_WhenRoleIsNull_ReturnsFalse()
    {
        // Arrange
        var memberService = new Mock<IDiscordMemberService>();
        var service = new ModerationActionsService(memberService.Object);
        var guild = DiscordObjectFactory.CreateUninitializedGuild(1);
        var member = DiscordObjectFactory.CreateMember(100);

        // Act
        bool result = await service.KickUserDueToRoleAssignmentAsync(guild, member, null, "reason");

        // Assert
        Assert.False(result);
        memberService.Verify(s => s.KickMemberAsync(It.IsAny<DSharpPlus.Entities.DiscordMember>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    internal async Task KickUserDueToRoleAssignmentAsync_WhenMemberLacksRole_ReturnsFalse()
    {
        // Arrange
        var memberService = new Mock<IDiscordMemberService>();
        var service = new ModerationActionsService(memberService.Object);
        var guild = DiscordObjectFactory.CreateUninitializedGuild(1);
        var member = DiscordObjectFactory.CreateMember(100);
        var role = DiscordObjectFactory.CreateRole(50);

        memberService.Setup(s => s.MemberHasRole(member, role)).Returns(false);

        // Act
        bool result = await service.KickUserDueToRoleAssignmentAsync(guild, member, role, "reason");

        // Assert
        Assert.False(result);
        memberService.Verify(s => s.KickMemberAsync(It.IsAny<DSharpPlus.Entities.DiscordMember>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    internal async Task KickUserDueToRoleAssignmentAsync_WhenMemberHasRoleAndIsKickedSuccessfully_ReturnsTrue()
    {
        // Arrange
        var memberService = new Mock<IDiscordMemberService>();
        var service = new ModerationActionsService(memberService.Object);
        var guild = DiscordObjectFactory.CreateUninitializedGuild(1);
        var member = DiscordObjectFactory.CreateMember(100, isPending: false);
        var role = DiscordObjectFactory.CreateRole(50);

        memberService.Setup(s => s.MemberHasRole(member, role)).Returns(true);
        memberService
            .Setup(s => s.KickMemberAsync(member, It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        bool result = await service.KickUserDueToRoleAssignmentAsync(guild, member, role, "reason");

        // Assert
        Assert.True(result);
        memberService.Verify(s => s.KickMemberAsync(member, "reason"), Times.Once);
    }

    [Fact]
    internal async Task KickUserAsync_WhenUserIsPending_ReturnsFalseWithoutKicking()
    {
        // Arrange
        var memberService = new Mock<IDiscordMemberService>();
        var service = new ModerationActionsService(memberService.Object);
        var guild = DiscordObjectFactory.CreateUninitializedGuild(1);
        var member = DiscordObjectFactory.CreateMember(100, isPending: true);

        // Act
        bool result = await service.KickUserAsync(guild, member, "reason");

        // Assert
        Assert.False(result);
        memberService.Verify(s => s.KickMemberAsync(It.IsAny<DSharpPlus.Entities.DiscordMember>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    internal async Task KickUserAsync_WhenKickThrowsException_ReturnsFalse()
    {
        // Arrange
        var memberService = new Mock<IDiscordMemberService>();
        var service = new ModerationActionsService(memberService.Object);
        var guild = DiscordObjectFactory.CreateUninitializedGuild(1);
        var member = DiscordObjectFactory.CreateMember(100, isPending: false);

        memberService
            .Setup(s => s.KickMemberAsync(member, It.IsAny<string>()))
            .ThrowsAsync(new Exception("Discord API error"));

        // Act
        bool result = await service.KickUserAsync(guild, member, "reason");

        // Assert
        Assert.False(result);
    }

    [Fact]
    internal async Task KickUserAsync_WhenKickSucceeds_ReturnsTrue()
    {
        // Arrange
        var memberService = new Mock<IDiscordMemberService>();
        var service = new ModerationActionsService(memberService.Object);
        var guild = DiscordObjectFactory.CreateUninitializedGuild(1);
        var member = DiscordObjectFactory.CreateMember(100, isPending: false);

        memberService
            .Setup(s => s.KickMemberAsync(member, It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        bool result = await service.KickUserAsync(guild, member, "reason");

        // Assert
        Assert.True(result);
        memberService.Verify(s => s.KickMemberAsync(member, "reason"), Times.Once);
    }

    [Fact]
    internal async Task KickUserDueToRoleAssignmentAsync_WhenMemberHasRoleButIsPending_ReturnsFalse()
    {
        // Arrange
        var memberService = new Mock<IDiscordMemberService>();
        var service = new ModerationActionsService(memberService.Object);
        var guild = DiscordObjectFactory.CreateUninitializedGuild(1);
        var member = DiscordObjectFactory.CreateMember(100, isPending: true);
        var role = DiscordObjectFactory.CreateRole(50);

        memberService.Setup(s => s.MemberHasRole(member, role)).Returns(true);

        // Act
        bool result = await service.KickUserDueToRoleAssignmentAsync(guild, member, role, "reason");

        // Assert
        Assert.False(result);
        memberService.Verify(s => s.KickMemberAsync(It.IsAny<DSharpPlus.Entities.DiscordMember>(), It.IsAny<string>()), Times.Never);
    }
}
