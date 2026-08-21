using LundBot.Utils;

namespace LundBot.Tests.Unit.Utilities;

public sealed class TimeUtilsTests
{
    [Fact]
    public void GetCurrentUtcWeekBounds_WhenGivenDate_ReturnsMondayAndNextMonday()
    {
        // Arrange
        DateTime reference = new(2026, 8, 19, 12, 0, 0, DateTimeKind.Utc);

        // Act
        (DateTime start, DateTime end) = TimeUtils.getCurrentUtcWeekBounds(reference);

        // Assert
        Assert.Equal(new DateTime(2026, 8, 17, 0, 0, 0, DateTimeKind.Utc), start);
        Assert.Equal(new DateTime(2026, 8, 24, 0, 0, 0, DateTimeKind.Utc), end);
    }
}
