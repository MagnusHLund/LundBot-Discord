using Microsoft.CodeAnalysis;

namespace LundBot.ArchitectureTests.Support;

internal static class ApiPayloadTypes
{
    public static IEnumerable<ITypeSymbol> ForbiddenTypes(ITypeSymbol type) =>
        ForbiddenTypes(type, new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default));

    private static IEnumerable<ITypeSymbol> ForbiddenTypes(ITypeSymbol type, HashSet<ITypeSymbol> visited)
    {
        if (!visited.Add(type))
        {
            yield break;
        }
        if (type is IArrayTypeSymbol array)
        {
            foreach (ITypeSymbol violation in ForbiddenTypes(array.ElementType, visited))
                yield return violation;
            yield break;
        }
        if (type is not INamedTypeSymbol named || type.SpecialType != SpecialType.None)
        {
            yield break;
        }

        string ns = named.ContainingNamespace.ToDisplayString();
        string name = named.OriginalDefinition.ToDisplayString();
        bool envelope = name is "System.Threading.Tasks.Task<TResult>" or "System.Threading.Tasks.ValueTask<TResult>"
            or "Microsoft.AspNetCore.Mvc.ActionResult<TValue>"
            or "System.Nullable<T>";
        bool collection = ns == "System.Collections.Generic" || name == "System.Threading.Tasks.Task";
        bool infrastructure = ns.StartsWith("LundBot.Domain", StringComparison.Ordinal)
            || ns.StartsWith("LundBot.Infrastructure", StringComparison.Ordinal)
            || ns.StartsWith("DSharpPlus", StringComparison.Ordinal)
            || ns.StartsWith("Microsoft", StringComparison.Ordinal);
        bool actionResult = name == "Microsoft.AspNetCore.Mvc.IActionResult";
        if (infrastructure && !envelope && !actionResult)
        {
            yield return named;
            yield break;
        }

        foreach (ITypeSymbol argument in named.TypeArguments)
        {
            foreach (ITypeSymbol violation in ForbiddenTypes(argument, visited))
                yield return violation;
        }

        if (!envelope && !collection && (named.IsAnonymousType || ns.StartsWith("LundBot.", StringComparison.Ordinal)))
        {
            foreach (IPropertySymbol property in named.GetMembers().OfType<IPropertySymbol>()
                .Where(property => property.DeclaredAccessibility == Accessibility.Public && !property.IsStatic))
            {
                foreach (ITypeSymbol violation in ForbiddenTypes(property.Type, visited))
                    yield return violation;
            }
            foreach (IFieldSymbol field in named.GetMembers().OfType<IFieldSymbol>()
                .Where(field => field.DeclaredAccessibility == Accessibility.Public && !field.IsStatic))
            {
                foreach (ITypeSymbol violation in ForbiddenTypes(field.Type, visited))
                    yield return violation;
            }
            if (named.BaseType is not null)
            {
                foreach (ITypeSymbol violation in ForbiddenTypes(named.BaseType, visited))
                    yield return violation;
            }
        }
    }
}
