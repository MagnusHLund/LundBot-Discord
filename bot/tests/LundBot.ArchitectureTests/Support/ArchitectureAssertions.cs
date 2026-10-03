using Xunit;

namespace LundBot.ArchitectureTests.Support;

internal static class ArchitectureAssertions
{
    public static void NoViolations(IEnumerable<string> violations)
    {
        string[] failures = violations.Distinct().Order(StringComparer.Ordinal).ToArray();
        Assert.True(failures.Length == 0, string.Join(Environment.NewLine, failures));
    }
}
