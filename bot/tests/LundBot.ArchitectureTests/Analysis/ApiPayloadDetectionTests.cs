using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

using LundBot.ArchitectureTests.Support;

namespace LundBot.ArchitectureTests.Analysis;

public sealed class ApiPayloadDetectionTests
{
    [Theory]
    [InlineData("LundBot.Domain.ArchitectureSamples.Payload", "Payload")]
    [InlineData("System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<System.Collections.Generic.List<LundBot.Domain.ArchitectureSamples.Payload>>>", "Payload")]
    [InlineData("DSharpPlus.Entities.DiscordUser", "DiscordUser")]
    public void Forbidden_types_are_detected_inside_DTO_members_and_response_envelopes(string payloadType, string expectedName)
    {
        INamedTypeSymbol contract = Contract(payloadType);

        ITypeSymbol[] violations = ApiPayloadTypes.ForbiddenTypes(contract).ToArray();

        Assert.Contains(violations, type => type.Name == expectedName);
    }

    [Theory]
    [InlineData("string")]
    [InlineData("System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<System.Collections.Generic.List<string>>>")]
    public void Serializable_payloads_and_MVC_envelopes_are_allowed(string payloadType)
    {
        INamedTypeSymbol contract = Contract(payloadType);

        ITypeSymbol[] violations = ApiPayloadTypes.ForbiddenTypes(contract).ToArray();

        Assert.Empty(violations);
    }

    private static INamedTypeSymbol Contract(string payloadType)
    {
        SemanticModel model = ArchitectureSourceSamples.Analyze($$"""
            namespace LundBot.Domain.ArchitectureSamples
            {
                public sealed class Payload { }
            }
            namespace LundBot.Presentation.ArchitectureSamples
            {
                public sealed class ApiContractSample
                {
                    public {{payloadType}} Value { get; } = default!;
                }
            }
            """);
        TypeDeclarationSyntax syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>()
            .Single(declaration => declaration.Identifier.ValueText == "ApiContractSample");
        return model.GetDeclaredSymbol(syntax) as INamedTypeSymbol
            ?? throw new InvalidOperationException("Could not resolve the sample API contract.");
    }
}
