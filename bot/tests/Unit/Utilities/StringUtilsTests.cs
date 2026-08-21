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
        const string input = "Map2Winner";

        // Act
        string result = StringUtils.SplitCamelCaseOrPascalCaseToWords(input);

        // Assert
        Assert.Equal("Map 2 winner", result);
    }
}
