using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

using LundBot.ArchitectureTests.Support;

namespace LundBot.ArchitectureTests.Analysis;

public sealed class DependencyDetectionTests
{
    [Theory]
    [InlineData("repository", "var repository = Create(); _ = repository;")]
    [InlineData("Create", "var repository = Create();")]
    [InlineData("CreateWrapped", "var repositories = CreateWrapped();")]
    public void Repository_dependencies_are_detected_in_inferred_locals_and_factory_returns(string identifier, string body)
    {
        SemanticModel model = ArchitectureSourceSamples.Analyze($$"""
            namespace LundBot.ArchitectureSamples;
            public interface IExampleRepository { }
            public sealed class DependencySample
            {
                private IExampleRepository Create() => throw new System.NotSupportedException();
                private System.Threading.Tasks.Task<IExampleRepository[]> CreateWrapped() => throw new System.NotSupportedException();
                public void Execute() { {{body}} }
            }
            """);
        MethodDeclarationSyntax method = model.SyntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()
            .Single(declaration => declaration.Identifier.ValueText == "Execute");
        SimpleNameSyntax reference = method.DescendantNodes().OfType<SimpleNameSyntax>()
            .First(node => node.Identifier.ValueText == identifier);

        INamedTypeSymbol[] dependencies = ArchitectureSymbols.ReferencedTypes(model.GetSymbolInfo(reference).Symbol).ToArray();

        Assert.Contains(dependencies, ArchitectureSymbols.IsRepository);
    }
}
