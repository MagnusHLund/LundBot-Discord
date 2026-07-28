namespace LundBot.Utils
{
    public static class TimeUtils
    {
        public static (DateTime, DateTime) getCurrentUtcWeekBounds(DateTime? referenceTime = null)
        {
            DateTime currentTime = referenceTime?.ToUniversalTime() ?? DateTime.UtcNow;

            DayOfWeek currentDay = currentTime.DayOfWeek;
            int dayFromMonday = ((int)currentDay + 6) % 7;

            DateTime startOfWeek = currentTime.AddDays(-dayFromMonday).Date;
            DateTime endOfWeek = startOfWeek.AddDays(7);

            return (startOfWeek, endOfWeek);
        }
    }
}
