namespace LundBot.Application.Common.Validation
{
    public static class ValidationUtils
    {
        private static readonly DateTime DiscordEpoch = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static bool IsValidDiscordId(ulong id)
        {
            // 1. Structural Minimum: Rejects values below Discord's absolute min bit-structure
            if (id < 4194304)
                return false;

            // 2. Extract the timestamp delta by shifting right by 22 bits
            ulong timestampDelta = id >> 22;

            // 3. Prevent potential overflow before adding to DateTime
            if (timestampDelta > double.MaxValue)
                return false;

            // 4. Calculate when this ID was actually generated
            DateTime generatedTime = DiscordEpoch.AddMilliseconds(timestampDelta);

            // 5. Time check: Must be generated after Discord launched,
            // and not in the future (allowing a 5-minute buffer for server clock drift)
            if (generatedTime < DiscordEpoch || generatedTime > DateTime.UtcNow.AddMinutes(5))
            {
                return false;
            }

            return true;
        }

        public static bool IsValidLengthString(string? input, int maxLength, int minLength = 0)
        {
            if (input is null)
            {
                return false;
            }

            int length = input.Length;
            return length >= minLength && length <= maxLength;
        }
    }
}
