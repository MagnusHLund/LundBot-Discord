using LundBot.Utils;

namespace LundBot.Tests.Unit.Utilities;

public sealed class StringUtilsTests
{
    [Fact]
    internal void SplitCamelCaseOrPascalCaseToWords_WhenPascalCase_ReturnsSpacedSentence()
    {
        // Arrange
        const string input = "CreateLeaderboard";

        // Act
        string result = StringUtils.SplitCamelCaseOrPascalCaseToWords(input);

        // Assert
        Assert.Equal("Create leaderboard", result);
    }

    [Fact]
    internal void SplitCamelCaseOrPascalCaseToWords_WhenContainsNumbers_ReturnsSpacedSentence()
    {
        // Arrange
        const string input = "IGot1NumberInMyString";

        // Act
        string result = StringUtils.SplitCamelCaseOrPascalCaseToWords(input);

        // Assert
        Assert.Equal("I got 1 number in my string", result);
    }
}
