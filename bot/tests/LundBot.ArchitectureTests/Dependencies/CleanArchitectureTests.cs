using System.Reflection;
using Xunit;

using LundBot.ArchitectureTests.Support;

namespace LundBot.ArchitectureTests.Dependencies;

public sealed class CleanArchitectureTests
{
    private static readonly Assembly _domainAssembly = typeof(LundBot.Domain.Leaderboards.Leaderboard).Assembly;
    private static readonly Assembly _applicationAssembly = typeof(LundBot.Application.DependencyInjection).Assembly;
    private static readonly Assembly _infrastructureAssembly = typeof(LundBot.Infrastructure.DependencyInjection).Assembly;
    private static readonly Assembly _presentationAssembly = typeof(LundBot.Presentation.Program).Assembly;

    [Fact]
    public void Domain_does_not_depend_on_outer_layers()
    {
        string[] outerLayerNames =
        [
            _applicationAssembly.GetName().Name!,
            _infrastructureAssembly.GetName().Name!,
            _presentationAssembly.GetName().Name!,
        ];

        AssertNoReferences(_domainAssembly, outerLayerNames);
    }

    [Fact]
    public void Application_does_not_depend_on_infrastructure_or_presentation()
    {
        string[] outerLayerNames =
        [
            _infrastructureAssembly.GetName().Name!,
            _presentationAssembly.GetName().Name!,
        ];

        AssertNoReferences(_applicationAssembly, outerLayerNames);
    }

    [Fact]
    public void Application_does_not_depend_on_framework_adapters()
    {
        string[] forbiddenPrefixes =
        [
            "Microsoft.EntityFrameworkCore",
            "Pomelo.",
            "DSharpPlus",
            "LundBot.Infrastructure",
            "LundBot.Presentation",
        ];
        string[] violations = _applicationAssembly
            .GetReferencedAssemblies()
            .Where(reference =>
                forbiddenPrefixes.Any(prefix => reference.Name?.StartsWith(prefix, StringComparison.Ordinal) == true)
            )
            .Select(reference => reference.Name!)
            .ToArray();

        Assert.True(
            violations.Length == 0,
            $"Application must depend on abstractions, not framework adapters:{Environment.NewLine}{string.Join(Environment.NewLine, violations)}"
        );
    }

    [Fact]
    public void Infrastructure_does_not_depend_on_presentation()
    {
        AssertNoReferences(_infrastructureAssembly, [_presentationAssembly.GetName().Name!]);
    }

    [Fact]
    public void Application_depends_on_domain_contracts()
    {
        Assert.Contains(
            _applicationAssembly.GetReferencedAssemblies(),
            reference => reference.Name == _domainAssembly.GetName().Name
        );
    }

    [Fact]
    public void Infrastructure_depends_on_application_and_domain_contracts()
    {
        AssemblyName[] references = _infrastructureAssembly.GetReferencedAssemblies().ToArray();

        Assert.Contains(references, reference => reference.Name == _applicationAssembly.GetName().Name);
        Assert.Contains(references, reference => reference.Name == _domainAssembly.GetName().Name);
    }

    [Fact]
    public void Domain_does_not_reference_persistence_or_discord_frameworks()
    {
        string[] forbiddenPrefixes = ["Microsoft.EntityFrameworkCore", "Pomelo.", "DSharpPlus"];
        string[] violations = _domainAssembly
            .GetReferencedAssemblies()
            .Where(reference => forbiddenPrefixes.Any(prefix => reference.Name?.StartsWith(prefix) == true))
            .Select(reference => reference.Name!)
            .ToArray();

        Assert.True(
            violations.Length == 0,
            $"Domain should remain independent of persistence and Discord frameworks:{Environment.NewLine}{string.Join(Environment.NewLine, violations)}"
        );
    }

    private static void AssertNoReferences(Assembly assembly, IEnumerable<string> forbiddenAssemblyNames)
    {
        HashSet<string> forbidden = forbiddenAssemblyNames.ToHashSet(StringComparer.Ordinal);
        string[] violations = assembly
            .GetReferencedAssemblies()
            .Where(reference => reference.Name is not null && forbidden.Contains(reference.Name))
            .Select(reference => reference.Name!)
            .ToArray();

        Assert.True(
            violations.Length == 0,
            $"{assembly.GetName().Name} must not reference: {string.Join(", ", violations)}"
        );
    }
}
