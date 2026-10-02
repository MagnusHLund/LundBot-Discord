using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

using LundBot.ArchitectureTests.Support;

namespace LundBot.ArchitectureTests.Api;

public sealed class ApiContractTests
{
    [Fact]
    public void HTTP_action_parameters_and_return_types_do_not_expose_domain_or_framework_types()
    {
        ArchitectureAssertions.NoViolations(Actions().SelectMany(entry =>
            entry.Method.Parameters.Select(parameter => parameter.Type).Append(entry.Method.ReturnType)
                .SelectMany(ApiPayloadTypes.ForbiddenTypes)
                .Select(type => $"{entry.Source.Location(entry.Declaration)} exposes {type}")));
    }

    [Fact]
    public void HTTP_response_payloads_do_not_hide_domain_or_framework_types_in_action_results()
    {
        ArchitectureAssertions.NoViolations(Actions().SelectMany(entry =>
            ResponseArguments(entry.Source, entry.Declaration)
                .Select(argument => entry.Source.Model.GetTypeInfo(argument.Expression).Type)
                .Where(type => type is not null)
                .SelectMany(type => ApiPayloadTypes.ForbiddenTypes(type!))
                .Select(type => $"{entry.Source.Location(entry.Declaration)} returns payload {type}")));
    }

    [Fact]
    public void API_DTO_members_do_not_expose_domain_or_framework_types()
    {
        ArchitectureAssertions.NoViolations(ProductionSourceCatalog.Sources
            .Where(source => source.Project == "LundBot.Presentation"
                && source.Path.StartsWith("Api/", StringComparison.Ordinal)
                && source.Path.Contains("/Dtos/", StringComparison.Ordinal))
            .SelectMany(source => source.Tree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>()
                .SelectMany(declaration =>
                    ApiPayloadTypes.ForbiddenTypes(source.DeclaredType(declaration))
                        .Select(type => $"{source.Location(declaration)} exposes {type}"))));
    }

    private static IEnumerable<ArgumentSyntax> ResponseArguments(ProductionSource source, MethodDeclarationSyntax declaration)
    {
        IEnumerable<ArgumentSyntax> helperArguments = declaration.DescendantNodes().OfType<InvocationExpressionSyntax>()
            .Where(call => source.Model.GetSymbolInfo(call).Symbol is IMethodSymbol method
                && method.ContainingType.ToDisplayString() == "Microsoft.AspNetCore.Mvc.ControllerBase"
                && new[] { "Ok", "Created", "CreatedAtAction", "CreatedAtRoute", "Accepted", "AcceptedAtAction",
                    "AcceptedAtRoute", "BadRequest", "NotFound", "Conflict", "UnprocessableEntity", "StatusCode",
                    "Json" }.Contains(method.Name))
            .SelectMany(call => call.ArgumentList.Arguments);
        IEnumerable<ArgumentSyntax> resultArguments = declaration.DescendantNodes().OfType<BaseObjectCreationExpressionSyntax>()
            .Where(creation => source.Model.GetTypeInfo(creation).Type is INamedTypeSymbol type
                && (ArchitectureSymbols.Inherits(type, "Microsoft.AspNetCore.Mvc.ObjectResult")
                    || type.ToDisplayString() == "Microsoft.AspNetCore.Mvc.JsonResult"))
            .SelectMany(creation => creation.ArgumentList?.Arguments ?? []);
        return helperArguments.Concat(resultArguments);
    }

    private static IEnumerable<(ProductionSource Source, MethodDeclarationSyntax Declaration, IMethodSymbol Method)> Actions() =>
        ProductionSourceCatalog.Sources
            .Where(source => source.Project == "LundBot.Presentation")
            .SelectMany(source => source.Tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Select(declaration => (Source: source, Declaration: declaration, Method: source.DeclaredMethod(declaration)))
                .Where(entry => entry.Method.DeclaredAccessibility == Accessibility.Public
                    && ArchitectureSymbols.Inherits(entry.Method.ContainingType, "Microsoft.AspNetCore.Mvc.ControllerBase")
                    && !entry.Method.GetAttributes().Any(attribute =>
                        attribute.AttributeClass?.ToDisplayString() == "Microsoft.AspNetCore.Mvc.NonActionAttribute")));

}
