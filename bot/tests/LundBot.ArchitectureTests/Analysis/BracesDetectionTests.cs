using LundBot.ArchitectureTests.Support;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace LundBot.ArchitectureTests.Analysis;

public sealed class BracesDetectionTests
{
    [Theory]
    [InlineData("if (value == null) return true; // Allow null values")]
    [InlineData("if (true) { } else return true;")]
    [InlineData("for (;;) break;")]
    [InlineData("foreach (var value in values) continue;")]
    [InlineData("foreach (var (key, value) in values) continue;")]
    [InlineData("while (true) break;")]
    [InlineData("do break; while (true);")]
    [InlineData("using (resource) return true;")]
    [InlineData("lock (resource) return true;")]
    [InlineData("fixed (int* p = values) return true;")]
    public void Unbraced_statement_body_is_detected(string code)
    {
        SyntaxNode root = Parse(code);

        var violations = BracesConvention.UnbracedBodies(root).ToArray();

        Assert.Single(violations);
    }

    [Theory]
    [InlineData("if (value == null) { return true; }")]
    [InlineData("if (true) { } else if (false) { } else { }")]
    [InlineData("for (;;) { break; }")]
    [InlineData("foreach (var value in values) { continue; }")]
    [InlineData("foreach (var (key, value) in values) { continue; }")]
    [InlineData("while (true) { break; }")]
    [InlineData("do { break; } while (true);")]
    [InlineData("using (resource) { return true; }")]
    [InlineData("using var resource = Create();")]
    [InlineData("lock (resource) { return true; }")]
    [InlineData("fixed (int* p = values) { return true; }")]
    public void Braced_bodies_and_else_if_chains_are_allowed(string code)
    {
        SyntaxNode root = Parse(code);

        var violations = BracesConvention.UnbracedBodies(root).ToArray();

        Assert.Empty(violations);
    }

    private static SyntaxNode Parse(string code)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText($$"""
            class Sample
            {
                bool Execute()
                {
                    {{code}}
                }
            }
            """);
        Assert.DoesNotContain(tree.GetDiagnostics(), diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        return tree.GetRoot();
    }
}
