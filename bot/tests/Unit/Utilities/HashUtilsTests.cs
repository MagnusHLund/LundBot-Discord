using LundBot.Utils;

namespace LundBot.Tests.Unit.Utilities;

public sealed class HashUtilsTests
{
    [Fact]
    internal void HashString_WhenInputProvided_ReturnsExpectedSha256Bytes()
    {
        // Arrange
        const string input = "LundBot";
        byte[] expected = Convert.FromHexString(
            "BC4203401C51F39760F90E061C30AEE31F92EF1E02E4C84732F9735CB0229080"
        );

        // Act
        byte[] actual = HashUtils.HashString(input);

        // Assert
        Assert.Equal(expected, actual);
    }
}
