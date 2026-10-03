using System.Reflection;
using System.Runtime.CompilerServices;

namespace LundBot.ArchitectureTests.Support;

internal static class ProductionTypeCatalog
{
    private static readonly Assembly[] _assemblies =
    [
        typeof(LundBot.Domain.Leaderboards.Leaderboard).Assembly,
        typeof(LundBot.Application.DependencyInjection).Assembly,
        typeof(LundBot.Infrastructure.DependencyInjection).Assembly,
        typeof(LundBot.Presentation.Program).Assembly,
    ];

    public static IEnumerable<Type> GetTypes() =>
        _assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type =>
                type.Namespace?.StartsWith("LundBot.", StringComparison.Ordinal) == true
                && !IsCompilerGenerated(type)
            );

    private static bool IsCompilerGenerated(Type type)
    {
        for (Type? current = type; current is not null; current = current.DeclaringType)
        {
            if (current.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false))
            {
                return true;
            }
        }

        return false;
    }
}
