using LundBot.Utils;

namespace LundBot.Tests.Unit.Utilities;

public sealed class HashUtilsTests
{
    [Fact]
    public void HashString_WhenInputProvided_ReturnsExpectedSha256Bytes()
    {
        // Arrange
        const string input = "lundbot";
        byte[] expected = Convert.FromHexString(
            "24BFC170869294EFC235665D46EB720722A3221108C6C02754593E8D7D5AB912"
        );

        // Act
        byte[] actual = HashUtils.HashString(input);

        // Assert
        Assert.Equal(expected, actual);
    }
}
