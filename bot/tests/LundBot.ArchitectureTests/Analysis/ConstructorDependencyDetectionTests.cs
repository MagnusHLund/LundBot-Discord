using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

using LundBot.ArchitectureTests.Support;

namespace LundBot.ArchitectureTests.Analysis;

public sealed class ConstructorDependencyDetectionTests
{
    [Theory]
    [InlineData("System.Net.Http.HttpClient")]
    [InlineData("LundBot.Application.Features.Moderation.IAutoKickRolesRepository")]
    public void Service_constructor_parameters_are_recognized_as_dependencies(string type)
    {
        IParameterSymbol parameter = Parameter(type);

        bool isDependency = ConstructorDependencies.IsDependency(parameter);

        Assert.True(isDependency);
    }

    [Theory]
    [InlineData("string")]
    [InlineData("int")]
    [InlineData("System.Threading.CancellationToken")]
    [InlineData("System.Collections.Generic.IReadOnlyList<string>")]
    [InlineData("string[]")]
    public void Constructor_data_is_not_mistaken_for_injected_services(string type)
    {
        IParameterSymbol parameter = Parameter(type);

        bool isDependency = ConstructorDependencies.IsDependency(parameter);

        Assert.False(isDependency);
    }

    private static IParameterSymbol Parameter(string type)
    {
        SemanticModel model = ArchitectureSourceSamples.Analyze($$"""
            namespace LundBot.ArchitectureSamples;
            public sealed class ConstructorSample
            {
                public ConstructorSample({{type}} dependency) { }
            }
            """);
        ParameterSyntax syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<ParameterSyntax>().Single();
        return model.GetDeclaredSymbol(syntax) as IParameterSymbol
            ?? throw new InvalidOperationException("Could not resolve the sample constructor parameter.");
    }
}
