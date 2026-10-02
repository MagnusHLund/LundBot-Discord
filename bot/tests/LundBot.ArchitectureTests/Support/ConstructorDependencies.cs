using Microsoft.CodeAnalysis;

namespace LundBot.ArchitectureTests.Support;

internal static class ConstructorDependencies
{
    public static bool IsDependency(IParameterSymbol parameter) =>
        parameter.ContainingSymbol is IMethodSymbol { MethodKind: MethodKind.Constructor }
        && !parameter.Type.IsValueType
        && parameter.Type.SpecialType != SpecialType.System_String
        && parameter.Type.TypeKind != TypeKind.Array
        && !parameter.Type.Name.EndsWith("Dto", StringComparison.Ordinal)
        && !parameter.Type.ContainingNamespace.ToDisplayString().StartsWith("System.Collections", StringComparison.Ordinal);
}
