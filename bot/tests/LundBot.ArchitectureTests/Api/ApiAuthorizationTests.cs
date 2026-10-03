using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xunit;

using LundBot.ArchitectureTests.Support;

namespace LundBot.ArchitectureTests.Api;

public sealed class ApiAuthorizationTests
{
    [Fact]
    public void Every_HTTP_action_declares_authorization_or_anonymous_access()
    {
        ArchitectureAssertions.NoViolations(
            ProductionTypeCatalog.GetTypes()
                .Where(type => !type.IsAbstract && typeof(ControllerBase).IsAssignableFrom(type))
                .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(method => method.DeclaringType?.Assembly == type.Assembly)
                    .Where(method => !method.IsSpecialName && !method.IsDefined(typeof(NonActionAttribute), inherit: true))
                    .Where(method =>
                        !method.GetCustomAttributes<AuthorizeAttribute>(inherit: true).Any()
                        && !method.GetCustomAttributes<AllowAnonymousAttribute>(inherit: true).Any()
                        && !type.GetCustomAttributes<AuthorizeAttribute>(inherit: true).Any()
                        && !type.GetCustomAttributes<AllowAnonymousAttribute>(inherit: true).Any())
                    .Select(method => $"{type.FullName}.{method.Name} must declare its authorization policy."))
        );
    }
}
