using LundBot.Utils;

namespace LundBot.Tests.Unit.Utilities;

public sealed class ValidationUtilsTests
{
    [Theory]
    [InlineData("abc", 3, 3)]
    [InlineData("abcd", 6, 2)]
    [InlineData("", 0, 0)]
    public void IsValidLengthString_WhenLengthWithinBounds_ReturnsTrue(
        string value,
        int maxLength,
        int minLength
    )
    {
        // Arrange

        // Act
        bool isValid = ValidationUtils.IsValidLengthString(value, maxLength, minLength);

        // Assert
        Assert.True(isValid);
    }

    [Theory]
    [InlineData("a", 10, 2)]
    [InlineData("abcdef", 5, 0)]
    public void IsValidLengthString_WhenLengthOutsideBounds_ReturnsFalse(
        string value,
        int maxLength,
        int minLength
    )
    {
        // Arrange

        // Act
        bool isValid = ValidationUtils.IsValidLengthString(value, maxLength, minLength);

        // Assert
        Assert.False(isValid);
    }
}
