using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit;

using LundBot.ArchitectureTests.Support;

namespace LundBot.ArchitectureTests.Conventions;

public sealed class AsyncMethodTests
{
    [Fact]
    public void Production_methods_do_not_use_async_void()
    {
        ArchitectureAssertions.NoViolations(ProductionTypeCatalog.GetTypes()
            .SelectMany(type => type.GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                | BindingFlags.Static | BindingFlags.DeclaredOnly))
            .Where(method => method.ReturnType == typeof(void)
                && method.IsDefined(typeof(AsyncStateMachineAttribute), inherit: false))
            .Select(method => $"{method.DeclaringType!.FullName}.{method.Name} must return Task or ValueTask."));
    }
}
