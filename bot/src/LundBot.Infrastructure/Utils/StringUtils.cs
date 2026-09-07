using System.Globalization;
using System.Text.RegularExpressions;

namespace LundBot.Infrastructure.Utils
{
    public static partial class StringUtils
    {
        [GeneratedRegex("(?<!^)(?=[A-Z0-9])", RegexOptions.Compiled)]
        private static partial Regex CamelCaseOrPascalCaseRegex();

        public static string SplitCamelCaseOrPascalCaseToWords(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            string spaced = CamelCaseOrPascalCaseRegex().Replace(input, " ");

            spaced = spaced.ToLowerInvariant();

            return char.ToUpper(spaced[0], CultureInfo.InvariantCulture) + spaced[1..];
        }
    }
}
