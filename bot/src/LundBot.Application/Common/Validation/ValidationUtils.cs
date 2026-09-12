namespace LundBot.Application.Common.Validation
{
    public static class ValidationUtils
    {
        private static readonly DateTime DiscordEpoch = new(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static bool IsValidDiscordId(ulong id)
        {
            // 1. Structural Minimum: Rejects values below Discord's absolute min bit-structure
            if (id < 4194304)
            {
                return false;
            }

            // 2. Extract the timestamp delta by shifting right by 22 bits
            ulong timestampDelta = id >> 22;

            // 3. Calculate when this ID was actually generated
            DateTime generatedTime = DiscordEpoch.AddMilliseconds(timestampDelta);

            // 4. Time check: Must be generated after Discord launched,
            // and not in the future (allowing a 5-minute buffer for server clock drift)
            return generatedTime <= DateTime.UtcNow.AddMinutes(5);
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
