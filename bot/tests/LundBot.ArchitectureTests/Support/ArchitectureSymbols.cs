using Microsoft.CodeAnalysis;

namespace LundBot.ArchitectureTests.Support;

internal static class ArchitectureSymbols
{
    public static bool IsRepository(INamedTypeSymbol type) =>
        type.Name.EndsWith("Repository", StringComparison.Ordinal)
        || type.AllInterfaces.Any(contract => contract.Name.EndsWith("Repository", StringComparison.Ordinal));

    public static bool Inherits(INamedTypeSymbol type, string baseType)
    {
        for (INamedTypeSymbol? current = type; current is not null; current = current.BaseType)
        {
            if (current.ToDisplayString() == baseType)
            {
                return true;
            }
        }
        return false;
    }

    public static IEnumerable<INamedTypeSymbol> ReferencedTypes(ISymbol? symbol)
    {
        ITypeSymbol? valueType = symbol switch
        {
            ITypeSymbol type => type,
            ILocalSymbol local => local.Type,
            IParameterSymbol parameter => parameter.Type,
            IFieldSymbol field => field.Type,
            IPropertySymbol property => property.Type,
            IMethodSymbol method => method.ReturnType,
            _ => null
        };

        if (symbol?.ContainingType is not null)
        {
            yield return symbol.ContainingType;
        }
        foreach (INamedTypeSymbol type in ExpandType(valueType))
        {
            yield return type;
        }
    }

    private static IEnumerable<INamedTypeSymbol> ExpandType(ITypeSymbol? type)
    {
        if (type is IArrayTypeSymbol array)
        {
            foreach (INamedTypeSymbol element in ExpandType(array.ElementType))
            {
                yield return element;
            }
        }
        if (type is INamedTypeSymbol named)
        {
            yield return named;
            foreach (INamedTypeSymbol argument in named.TypeArguments.SelectMany(ExpandType))
            {
                yield return argument;
            }
        }
    }
}
