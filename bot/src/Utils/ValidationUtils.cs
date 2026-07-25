namespace LundBot.Utils
{
    public static class ValidationUtils
    {
        public static bool IsValidLengthString(string input, int maxLength, int minLength = 0)
        {
            int length = input.Length;
            return length >= minLength && length <= maxLength;
        }
    }
}
