using LundBot.ArchitectureTests.Support;
using Xunit;

namespace LundBot.ArchitectureTests.Conventions;

public sealed class TypeConventionTests
{
    [Fact]
    public void Interfaces_are_prefixed_with_I()
    {
        string[] violations = ProductionTypeCatalog
            .GetTypes()
            .Where(type => type.IsInterface && !type.Name.StartsWith('I'))
            .Select(type => type.FullName!)
            .ToArray();

        Assert.True(
            violations.Length == 0,
            $"Interfaces must start with 'I':{Environment.NewLine}{Format(violations)}"
        );
    }

    [Fact]
    public void Abstract_classes_are_prefixed_with_Abstract()
    {
        string[] violations = ProductionTypeCatalog
            .GetTypes()
            .Where(type =>
                type.IsClass
                && type.IsAbstract
                && !type.IsSealed
                && !type.Name.StartsWith("Abstract", StringComparison.Ordinal)
            )
            .Select(type => type.FullName!)
            .ToArray();

        Assert.True(
            violations.Length == 0,
            $"Abstract classes must start with 'Abstract':{Environment.NewLine}{Format(violations)}"
        );
    }

    [Fact]
    public void Concrete_classes_including_records_without_inheritors_are_sealed()
    {
        Type[] types = ProductionTypeCatalog
            .GetTypes()
            .Where(type => type.IsClass && !IsGeneratedMigration(type))
            .ToArray();
        string[] violations = types
            .Where(type =>
                !type.IsAbstract
                && !type.IsSealed
                && !types.Any(candidate => candidate != type && type.IsAssignableFrom(candidate))
            )
            .Select(type => type.FullName!)
            .ToArray();

        Assert.True(
            violations.Length == 0,
            $"Concrete classes, including records, without inheritors must be sealed:{Environment.NewLine}{Format(violations)}"
        );
    }

    private static bool IsGeneratedMigration(Type type) =>
        type.Namespace?.Contains(".Persistence.Migrations", StringComparison.Ordinal) ?? false;

    private static string Format(IEnumerable<string> violations) => string.Join(Environment.NewLine, violations);
}
