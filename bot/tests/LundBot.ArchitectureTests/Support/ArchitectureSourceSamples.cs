using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LundBot.ArchitectureTests.Support;

internal static class ArchitectureSourceSamples
{
    public static SemanticModel Analyze(string code)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
        Compilation compilation = ProductionSourceCatalog.Sources
            .First(source => source.Project == "LundBot.Presentation")
            .Model.Compilation.AddSyntaxTrees(tree);
        Diagnostic[] errors = compilation.GetDiagnostics()
            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .ToArray();
        if (errors.Length > 0)
        {
            throw new InvalidOperationException(string.Join(Environment.NewLine, errors.Select(error => error.ToString())));
        }
        return compilation.GetSemanticModel(tree);
    }
}
