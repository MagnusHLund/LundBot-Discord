using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Xunit;

using LundBot.ArchitectureTests.Support;

namespace LundBot.ArchitectureTests.Conventions;

public sealed class FieldConventionTests
{
    [Fact]
    public void Private_fields_use_underscore_prefixed_camel_case()
    {
        string[] violations = ProductionTypeCatalog.GetTypes()
            .SelectMany(type => type.GetFields(
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly
            ))
            .Where(field => field.IsPrivate && !field.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false))
            .Where(field => !Regex.IsMatch(field.Name, "^_[a-z][a-zA-Z0-9]*$"))
            .Select(field => $"{field.DeclaringType!.FullName}.{field.Name}")
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            violations.Length == 0,
            $"Private fields must use '_camelCase':{Environment.NewLine}{string.Join(Environment.NewLine, violations)}"
        );
    }
}
