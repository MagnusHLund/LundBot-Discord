using LundBot.Utils;

namespace LundBot.Tests.Unit.Utilities;

public sealed class StringUtilsTests
{
    [Fact]
    public void SplitCamelCaseOrPascalCaseToWords_WhenPascalCase_ReturnsSpacedSentence()
    {
        // Arrange
        const string input = "CreateLeaderboard";

        // Act
        string result = StringUtils.SplitCamelCaseOrPascalCaseToWords(input);

        // Assert
        Assert.Equal("Create leaderboard", result);
    }

    [Fact]
    public void SplitCamelCaseOrPascalCaseToWords_WhenContainsNumbers_ReturnsSpacedSentence()
    {
        // Arrange
        const string input = "IGot1NumberInMyString";

        // Act
        string result = StringUtils.SplitCamelCaseOrPascalCaseToWords(input);

        // Assert
        Assert.Equal("I got 1 number in my string", result);
    }
}
