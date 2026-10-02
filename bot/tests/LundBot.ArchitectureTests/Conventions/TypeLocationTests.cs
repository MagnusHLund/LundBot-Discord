using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

using LundBot.ArchitectureTests.Support;

namespace LundBot.ArchitectureTests.Conventions;

public sealed class TypeLocationTests
{
    [Fact]
    public void Type_names_match_their_source_file_names()
    {
        ArchitectureAssertions.NoViolations(Declarations()
            .Where(entry => Path.GetFileNameWithoutExtension(entry.Source.Path) != entry.Type.Name)
            .Select(entry => $"{entry.Source.Location(entry.Declaration)} should be named {entry.Type.Name}.cs."));
    }

    [Fact]
    public void Type_namespaces_match_their_project_and_feature_folder()
    {
        ArchitectureAssertions.NoViolations(Declarations()
            .Where(entry => entry.Type.ContainingNamespace.ToDisplayString() !=
                entry.Source.Project + (entry.Source.Path.Contains('/')
                    ? "." + entry.Source.Path[..entry.Source.Path.LastIndexOf('/')].Replace('/', '.')
                    : string.Empty))
            .Select(entry => $"{entry.Source.Location(entry.Declaration)} has namespace {entry.Type.ContainingNamespace}."));
    }

    [Fact]
    public void DTOs_use_Dto_suffix_and_live_in_API_Dtos_or_Application_Discord()
    {
        ArchitectureAssertions.NoViolations(Declarations()
            .Where(entry => entry.Type.Name.EndsWith("Dto", StringComparison.Ordinal)
                || entry.Type.ContainingNamespace.ToDisplayString().EndsWith(".Dtos", StringComparison.Ordinal))
            .Where(entry => !entry.Type.Name.EndsWith("Dto", StringComparison.Ordinal)
                || !(entry.Source.Project == "LundBot.Application" && entry.Source.Path.StartsWith("Discord/", StringComparison.Ordinal)
                    || entry.Source.Project == "LundBot.Presentation" && entry.Source.Path.StartsWith("Api/", StringComparison.Ordinal)
                        && entry.Source.Path.Contains("/Dtos/", StringComparison.Ordinal)))
            .Select(entry => $"{entry.Source.Location(entry.Declaration)} violates the DTO name/location convention."));
    }

    [Fact]
    public void Repository_implementations_live_in_persistence_feature_folders()
    {
        ArchitectureAssertions.NoViolations(Declarations()
            .Where(entry => entry.Type.TypeKind == TypeKind.Class && ArchitectureSymbols.IsRepository(entry.Type))
            .Where(entry => !entry.Type.Name.EndsWith("Repository", StringComparison.Ordinal)
                || entry.Source.Project != "LundBot.Infrastructure"
                || !entry.Source.Path.StartsWith("Persistence/Repositories/", StringComparison.Ordinal))
            .Select(entry => $"{entry.Source.Location(entry.Declaration)} violates the repository name/location convention."));
    }

    [Fact]
    public void Repository_contracts_use_I_Repository_names_and_live_with_Application_features_or_common_messaging()
    {
        ArchitectureAssertions.NoViolations(Declarations()
            .Where(entry => entry.Type.TypeKind == TypeKind.Interface && ArchitectureSymbols.IsRepository(entry.Type))
            .Where(entry => !entry.Type.Name.StartsWith('I')
                || !entry.Type.Name.EndsWith("Repository", StringComparison.Ordinal)
                || entry.Source.Project != "LundBot.Application"
                || !(entry.Source.Path.StartsWith("Features/", StringComparison.Ordinal)
                    || entry.Source.Path.StartsWith("Common/Messaging/", StringComparison.Ordinal)))
            .Select(entry => $"{entry.Source.Location(entry.Declaration)} violates the repository contract convention."));
    }

    [Fact]
    public void Configuration_types_use_Config_suffix_and_live_in_configuration_folders()
    {
        ArchitectureAssertions.NoViolations(Declarations()
            .Where(entry => entry.Type.Name.EndsWith("Config", StringComparison.Ordinal)
                || entry.Source.Path.StartsWith("Config/", StringComparison.Ordinal)
                || entry.Source.Path.Contains("/Configuration/", StringComparison.Ordinal))
            .Where(entry => !entry.Type.Name.EndsWith("Config", StringComparison.Ordinal)
                || !(entry.Source.Project == "LundBot.Presentation" && entry.Source.Path.StartsWith("Config/", StringComparison.Ordinal)
                    || entry.Source.Project == "LundBot.Infrastructure" && entry.Source.Path.StartsWith("Discord/Configuration/", StringComparison.Ordinal)))
            .Select(entry => $"{entry.Source.Location(entry.Declaration)} violates the configuration name/location convention."));
    }

    [Fact]
    public void Slash_command_types_use_Command_suffix_and_live_in_Presentation_Discord()
    {
        ArchitectureAssertions.NoViolations(Declarations()
            .Where(entry => ArchitectureSymbols.Inherits(entry.Type, "LundBot.Presentation.Discord.Bot.AbstractBaseCommand")
                || entry.Type.GetMembers().OfType<IMethodSymbol>().Any(method =>
                method.GetAttributes().Any(attribute => attribute.AttributeClass?.ToDisplayString() == "DSharpPlus.Commands.CommandAttribute")))
            .Where(entry => !entry.Type.Name.EndsWith("Command", StringComparison.Ordinal)
                || entry.Source.Project != "LundBot.Presentation"
                || !entry.Source.Path.StartsWith("Discord/", StringComparison.Ordinal))
            .Select(entry => $"{entry.Source.Location(entry.Declaration)} violates the slash command name/location convention."));
    }

    private static IEnumerable<(ProductionSource Source, TypeDeclarationSyntax Declaration, INamedTypeSymbol Type)> Declarations() =>
        ProductionSourceCatalog.Sources.SelectMany(source =>
            source.Tree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>()
                .Where(declaration => declaration.Parent is not TypeDeclarationSyntax)
                .Select(declaration => (Source: source, Declaration: declaration, Type: source.DeclaredType(declaration))));
}
