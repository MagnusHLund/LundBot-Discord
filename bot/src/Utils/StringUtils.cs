using System.Globalization;
using System.Text.RegularExpressions;

namespace LundBot.Utils
{
    public static class StringUtils
    {
        public static string SplitCamelCaseOrPascalCaseToWords(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            string spaced = Regex.Replace(input, @"(?<!^)(?=[A-Z0-9])", " ");

            spaced = spaced.ToLowerInvariant();

            return char.ToUpper(spaced[0], CultureInfo.InvariantCulture) + spaced[1..];
        }
    }
}
