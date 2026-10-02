using Xunit;

using LundBot.ArchitectureTests.Support;

namespace LundBot.ArchitectureTests.Contracts;

public sealed class ServiceContractTests
{
    [Fact]
    public void Application_service_interfaces_do_not_expose_IQueryable()
    {
        ArchitectureAssertions.NoViolations(ProductionTypeCatalog.GetTypes()
            .Where(type => type.IsInterface
                && type.Assembly.GetName().Name == "LundBot.Application"
                && type.Name.Split('`')[0].EndsWith("Service", StringComparison.Ordinal))
            .SelectMany(type => type.GetInterfaces().Append(type))
            .Distinct()
            .SelectMany(type => type.GetMethods().SelectMany(method =>
                method.GetParameters().Select(parameter => parameter.ParameterType).Append(method.ReturnType)
                    .Where(ContainsQueryable)
                    .Select(contractType => $"{type.FullName}.{method.Name} exposes {contractType}"))));
    }

    private static bool ContainsQueryable(Type type) => ContainsQueryable(type, []);

    private static bool ContainsQueryable(Type type, HashSet<Type> visited)
    {
        if (!visited.Add(type))
        {
            return false;
        }

        if (typeof(IQueryable).IsAssignableFrom(type)
            || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryable<>))
            || (type.HasElementType && ContainsQueryable(type.GetElementType()!, visited))
            || type.GenericTypeArguments.Any(argument => ContainsQueryable(argument, visited)))
        {
            return true;
        }

        return type.Namespace?.StartsWith("LundBot.", StringComparison.Ordinal) == true
            && (type.GetProperties().Any(property => ContainsQueryable(property.PropertyType, visited))
                || type.GetFields().Any(field => ContainsQueryable(field.FieldType, visited)));
    }
}
