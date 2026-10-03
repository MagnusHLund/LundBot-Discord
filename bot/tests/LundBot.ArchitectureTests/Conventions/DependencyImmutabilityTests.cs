using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

using LundBot.ArchitectureTests.Support;

namespace LundBot.ArchitectureTests.Conventions;

public sealed class DependencyImmutabilityTests
{
    [Fact]
    public void Fields_assigned_constructor_dependencies_are_readonly()
    {
        ArchitectureAssertions.NoViolations(ProductionSourceCatalog.Sources.SelectMany(source =>
            source.Tree.GetRoot().DescendantNodes().OfType<AssignmentExpressionSyntax>()
                .Where(assignment => assignment.Ancestors().OfType<ConstructorDeclarationSyntax>().Any())
                .Where(assignment => source.Model.GetSymbolInfo(assignment.Left).Symbol is IFieldSymbol { IsReadOnly: false })
                .Where(assignment => assignment.Right.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>()
                    .Any(identifier => source.Model.GetSymbolInfo(identifier).Symbol is IParameterSymbol parameter
                        && ConstructorDependencies.IsDependency(parameter)))
                .Select(assignment => $"{source.Location(assignment)} stores a constructor dependency in a mutable field.")));
    }

    [Fact]
    public void Fields_initialized_from_primary_constructor_dependencies_are_readonly()
    {
        ArchitectureAssertions.NoViolations(ProductionSourceCatalog.Sources.SelectMany(source =>
            source.Tree.GetRoot().DescendantNodes().OfType<VariableDeclaratorSyntax>()
                .Where(variable => source.Model.GetDeclaredSymbol(variable) is IFieldSymbol { IsReadOnly: false })
                .Where(variable => variable.Initializer?.Value.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>()
                    .Any(identifier => source.Model.GetSymbolInfo(identifier).Symbol is IParameterSymbol parameter
                        && ConstructorDependencies.IsDependency(parameter)) == true)
                .Select(variable => $"{source.Location(variable)} stores a primary-constructor dependency in a mutable field.")));
    }

    [Fact]
    public void Primary_constructor_dependencies_are_not_captured_as_mutable_state()
    {
        ArchitectureAssertions.NoViolations(ProductionSourceCatalog.Sources.SelectMany(source =>
            source.Tree.GetRoot().DescendantNodes().OfType<IdentifierNameSyntax>()
                .Where(identifier => source.Model.GetSymbolInfo(identifier).Symbol is IParameterSymbol parameter
                    && ConstructorDependencies.IsDependency(parameter)
                    && parameter.DeclaringSyntaxReferences.Any(reference =>
                        reference.GetSyntax().Parent?.Parent is TypeDeclarationSyntax))
                .Where(identifier => !identifier.Ancestors().OfType<FieldDeclarationSyntax>().Any()
                    && !identifier.Ancestors().OfType<PrimaryConstructorBaseTypeSyntax>().Any()
                    && !identifier.Ancestors().OfType<ConstructorDeclarationSyntax>().Any())
                .Select(identifier => $"{source.Location(identifier)} captures a primary-constructor dependency; use a readonly field.")));
    }

    [Fact]
    public void Constructor_dependencies_are_not_stored_in_properties()
    {
        ArchitectureAssertions.NoViolations(ProductionSourceCatalog.Sources.SelectMany(source =>
            source.Tree.GetRoot().DescendantNodes().OfType<AssignmentExpressionSyntax>()
                .Where(assignment => assignment.Ancestors().OfType<ConstructorDeclarationSyntax>().Any())
                .Where(assignment => source.Model.GetSymbolInfo(assignment.Left).Symbol is IPropertySymbol)
                .Where(assignment => assignment.Right.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>()
                    .Any(identifier => source.Model.GetSymbolInfo(identifier).Symbol is IParameterSymbol parameter
                        && ConstructorDependencies.IsDependency(parameter)))
                .Select(assignment => $"{source.Location(assignment)} stores an injected service in a property; use a readonly field.")));
    }
}
