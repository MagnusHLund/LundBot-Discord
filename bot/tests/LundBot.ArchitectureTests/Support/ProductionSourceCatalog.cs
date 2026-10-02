using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LundBot.ArchitectureTests.Support;

internal static class ProductionSourceCatalog
{
    private static readonly Lazy<IReadOnlyList<ProductionSource>> _sources = new(LoadSources);

    public static IReadOnlyList<ProductionSource> Sources => _sources.Value;

    private static IReadOnlyList<ProductionSource> LoadSources()
    {
        string root = Path.Combine(AppContext.BaseDirectory, "ArchitectureSources");
        if (!Directory.Exists(root))
        {
            throw new InvalidOperationException($"Architecture source files were not copied to {root}.");
        }

        string[] runtimePaths = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
            ?? throw new InvalidOperationException("Runtime assembly references are unavailable."))
            .Split(Path.PathSeparator);
        string[] assemblyPaths = runtimePaths
            .Concat(Directory.GetFiles(AppContext.BaseDirectory, "*.dll"))
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        List<ProductionSource> sources = [];

        foreach (string project in new[] { "LundBot.Domain", "LundBot.Application", "LundBot.Infrastructure", "LundBot.Presentation" })
        {
            string[] paths = Directory.GetFiles(Path.Combine(root, project), "*.cs", SearchOption.AllDirectories)
                .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}Migrations{Path.DirectorySeparatorChar}"))
                .Order(StringComparer.Ordinal)
                .ToArray();
            if (paths.Length == 0)
            {
                throw new InvalidOperationException($"No architecture source files found for {project}.");
            }

            SyntaxTree[] trees = paths.Select(path =>
                CSharpSyntaxTree.ParseText(File.ReadAllText(path), path: path)
            ).ToArray();
            SyntaxTree implicitUsings = CSharpSyntaxTree.ParseText("""
                global using System;
                global using System.Collections.Generic;
                global using System.IO;
                global using System.Linq;
                global using System.Net.Http;
                global using System.Threading;
                global using System.Threading.Tasks;
                """);
            IEnumerable<SyntaxTree> allTrees = trees.Append(implicitUsings);
            if (project == "LundBot.Presentation")
            {
                allTrees = allTrees.Append(CSharpSyntaxTree.ParseText("""
                    global using Microsoft.AspNetCore.Builder;
                    global using Microsoft.AspNetCore.Hosting;
                    global using Microsoft.AspNetCore.Http;
                    global using Microsoft.AspNetCore.Routing;
                    global using Microsoft.Extensions.Configuration;
                    global using Microsoft.Extensions.DependencyInjection;
                    global using Microsoft.Extensions.Hosting;
                    global using Microsoft.Extensions.Logging;
                    """));
            }

            MetadataReference[] references = assemblyPaths
                .Where(path => Path.GetFileNameWithoutExtension(path) != project)
                .Select(path => MetadataReference.CreateFromFile(path))
                .ToArray();
            CSharpCompilation compilation = CSharpCompilation.Create(
                project,
                allTrees,
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );
            Diagnostic[] errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
            if (errors.Length > 0)
            {
                throw new InvalidOperationException(
                    $"Cannot analyze {project}:{Environment.NewLine}{string.Join(Environment.NewLine, errors.Select(e => e.ToString()))}"
                );
            }

            sources.AddRange(trees.Select(tree => new ProductionSource(
                project,
                Path.GetRelativePath(Path.Combine(root, project), tree.FilePath).Replace('\\', '/'),
                tree,
                compilation.GetSemanticModel(tree)
            )));
        }

        return sources;
    }
}

internal sealed record ProductionSource(string Project, string Path, SyntaxTree Tree, SemanticModel Model)
{
    public INamedTypeSymbol DeclaredType(TypeDeclarationSyntax declaration) =>
        Model.GetDeclaredSymbol(declaration) as INamedTypeSymbol
        ?? throw new InvalidOperationException($"Cannot resolve type at {Location(declaration)}.");

    public IMethodSymbol DeclaredMethod(MethodDeclarationSyntax declaration) =>
        Model.GetDeclaredSymbol(declaration) as IMethodSymbol
        ?? throw new InvalidOperationException($"Cannot resolve method at {Location(declaration)}.");

    public string Location(SyntaxNode node) =>
        $"{Project}/{Path}:{node.GetLocation().GetLineSpan().StartLinePosition.Line + 1}";
}
