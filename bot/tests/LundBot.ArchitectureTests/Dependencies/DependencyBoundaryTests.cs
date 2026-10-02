using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

using LundBot.ArchitectureTests.Support;

namespace LundBot.ArchitectureTests.Dependencies;

public sealed class DependencyBoundaryTests
{
    [Fact]
    public void Presentation_does_not_reference_infrastructure_outside_Program()
    {
        ArchitectureAssertions.NoViolations(
            ProductionSourceCatalog.Sources
                .Where(source => source.Project == "LundBot.Presentation" && source.Path != "Program.cs")
                .SelectMany(source => ReferencedTypes(source)
                    .Where(reference => reference.Type.ContainingNamespace.ToDisplayString()
                        .StartsWith("LundBot.Infrastructure", StringComparison.Ordinal))
                    .Select(reference => $"{source.Location(reference.Node)} references {reference.Type}"))
        );
    }

    [Fact]
    public void Controllers_and_commands_do_not_reference_repositories_or_DbContext()
    {
        ArchitectureAssertions.NoViolations(
            ProductionSourceCatalog.Sources.SelectMany(source =>
                source.Tree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>()
                    .Where(declaration => source.Model.GetDeclaredSymbol(declaration) is INamedTypeSymbol type
                        && (ArchitectureSymbols.Inherits(type, "Microsoft.AspNetCore.Mvc.ControllerBase")
                            || ArchitectureSymbols.Inherits(type, "LundBot.Presentation.Discord.Bot.AbstractBaseCommand")))
                    .SelectMany(declaration => ReferencedTypes(source, declaration)
                        .Where(reference => ArchitectureSymbols.IsRepository(reference.Type)
                            || ArchitectureSymbols.Inherits(reference.Type, "Microsoft.EntityFrameworkCore.DbContext"))
                        .Select(reference => $"{source.Location(reference.Node)} references {reference.Type}")))
        );
    }

    private static IEnumerable<(SyntaxNode Node, INamedTypeSymbol Type)> ReferencedTypes(
        ProductionSource source,
        SyntaxNode? root = null
    ) =>
        (root ?? source.Tree.GetRoot()).DescendantNodes().OfType<SimpleNameSyntax>()
            .SelectMany(node => ArchitectureSymbols.ReferencedTypes(source.Model.GetSymbolInfo(node).Symbol)
                .Select(type => (Node: (SyntaxNode)node, Type: type)));
}
