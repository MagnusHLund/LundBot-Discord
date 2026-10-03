using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

using LundBot.ArchitectureTests.Support;

namespace LundBot.ArchitectureTests.Dependencies;

public sealed class RepositoryPlacementTests
{
    [Fact]
    public void Repository_contracts_belong_in_Application()
    {
        ArchitectureAssertions.NoViolations(RepositoryTypes()
            .Where(entry => entry.Type.TypeKind == TypeKind.Interface && entry.Source.Project != "LundBot.Application")
            .Select(entry => $"{entry.Type} must be declared in Application."));
    }

    [Fact]
    public void Repository_implementations_belong_in_Infrastructure()
    {
        ArchitectureAssertions.NoViolations(RepositoryTypes()
            .Where(entry => entry.Type.TypeKind == TypeKind.Class && entry.Source.Project != "LundBot.Infrastructure")
            .Select(entry => $"{entry.Type} must be declared in Infrastructure."));
    }

    [Fact]
    public void Repository_implementations_implement_an_Application_repository_contract()
    {
        ArchitectureAssertions.NoViolations(RepositoryTypes()
            .Where(entry => entry.Type.TypeKind == TypeKind.Class && !entry.Type.IsAbstract)
            .Where(entry => !entry.Type.AllInterfaces.Any(contract =>
                contract.Name.EndsWith("Repository", StringComparison.Ordinal)
                && contract.ContainingAssembly.Name == "LundBot.Application"))
            .Select(entry => $"{entry.Type} must implement an Application repository interface."));
    }

    private static IEnumerable<(ProductionSource Source, INamedTypeSymbol Type)> RepositoryTypes() =>
        ProductionSourceCatalog.Sources.SelectMany(source =>
            source.Tree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>()
                .Select(declaration => (Source: source, Type: source.DeclaredType(declaration)))
                .Where(entry => ArchitectureSymbols.IsRepository(entry.Type)));
}
