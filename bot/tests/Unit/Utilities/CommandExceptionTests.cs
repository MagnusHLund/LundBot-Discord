using LundBot.Commands;
using LundBot.Exceptions;

namespace LundBot.Tests.Unit.Utilities;

public sealed class CommandExceptionTests
{
    [Fact]
    public void GetMessage_WhenShowMessageToUserTrue_ReturnsOriginalMessage()
    {
        // Arrange
        CommandException ex = new("Visible", showMessageToUser: true);

        // Act
        string message = ex.GetMessage();

        // Assert
        Assert.Equal("Visible", message);
    }

    [Fact]
    public void GetMessage_WhenShowMessageToUserFalse_ReturnsGenericMessage()
    {
        // Arrange
        CommandException ex = new("Hidden");

        // Act
        string message = ex.GetMessage();

        // Assert
        Assert.Equal(BaseCommand.GENERIC_ERROR_MESSAGE, message);
    }
}
