---
description: This file describes the instructions for the .NET LundBot Discord Bot project.
applyTo: '/bot/**'
---

# LundBot Discord Bot - Copilot instructions

## C# and .NET

This project is written in .NET 10 with the latest C# version.
Prefer modern language features when they improve readability and maintainability.
Do not use language features unavailable in the project's target framework.
Do not use the latest language features, if they make the code less readable.

Prefer writing generic, reusable code. However it should not be over-engineered. Avoid unnecessary abstractions and complexity.

Generally prefer using best practices and patterns that are widely accepted in the .NET community, unless there is a specific reason to deviate from them.

Prefer using the built-in .NET libraries and features over third-party libraries, unless there is a specific reason to use a third-party library.

## Priorities

## Priorities

The following priorities are ordered from highest to lowest importance. When two priorities conflict, prefer the higher-priority one.

1. Correctness and functional behavior.
2. Security and protection of data.
3. Reliability and resilience.
4. Readability and maintainability of the code.
5. Performance and efficiency.
6. Testability and observability.
7. Consistency with the existing architecture and conventions.
8. Developer experience and ease of use.
9. Minimizing unnecessary complexity and dependencies.
10. Minimizing the size and scope of changes.

Higher priorities take precedence over lower priorities. However, do not unnecessarily sacrifice a lower priority when doing so provides no meaningful benefit to a higher priority.

## Architecture, Patterns & Design Principles

The project follows the following architectural principles, patterns, and organizational conventions:

1. [SOLID](https://en.wikipedia.org/wiki/SOLID) principles.
   - Prefer clear separation of responsibilities.
   - Favor composition over inheritance where appropriate.
   - Keep abstractions focused and meaningful.
   - Do not introduce abstractions solely to satisfy a principle when they provide no practical benefit.

2. [Dependency Injection](https://en.wikipedia.org/wiki/Dependency_injection).
   - Use the built-in .NET dependency injection container.
   - Dependencies should generally be provided through constructors.
   - Avoid service locator patterns and manually resolving dependencies from `IServiceProvider` outside of appropriate composition-root scenarios.

3. [Clean Architecture](https://prepstack.co.in/blog/clean-architecture-csharp-complete-guide).
   - Keep business and application logic independent of infrastructure and presentation concerns.
   - Dependencies should point toward the appropriate inner layers.
   - Infrastructure-specific concerns such as databases, HTTP clients, and Discord APIs should not leak into domain logic.
   - Follow the existing project boundaries rather than introducing new architectural layers for individual features.

4. [Repository](https://martinfowler.com/eaaCatalog/repository.html) pattern.
   - Repositories are responsible for persistence and data-access concerns.
   - Business logic should remain outside repositories.
   - Do not place Discord, HTTP, or presentation concerns in repositories.

5. [Arrange-Act-Assert](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices#arrange-act-assert) pattern for automated tests.
   - Structure tests into clear Arrange, Act, and Assert phases.
   - Keep tests deterministic and focused on observable behavior.

6. [Feature Folders](https://milanjovanovic.tech/blog/feature-folders-dotnet) for organizing code by feature rather than by technical layer.
   - Prefer locating related code close together within its feature.
   - Do not reorganize code into separate global folders such as `Controllers`, `Services`, and `Repositories` when the existing feature-oriented structure provides a better organization.
   - Follow the existing feature structure when adding new functionality.

## Classes & Methods

Methods should have a clear responsibility.
Avoid methods that perform unrelated operations.
Prefer composition over large classes with many responsibilities.
Avoid deeply nested control flow.
Prefer early returns where they improve readability.

## Naming conventions

Use the standard C# naming conventions for classes, methods, properties, and variables.

- PascalCase for types and public members.
- camelCase for local variables and parameters.
- `_camelCase` for private fields if consistent with existing code.
- Async methods end with `Async`.
- Interfaces begin with `I`.
- Boolean properties should use meaningful prefixes such as `Is`, `Has`, or `Can` where appropriate.

## Async & Await

Use asynchronous APIs for I/O-bound operations.
Do not use `.Result` or `.Wait()` on tasks.
Do not use `Task.Run` merely to make synchronous I/O appear asynchronous.

Propagate cancellation tokens where appropriate.

Use `ConfigureAwait(false)` only when appropriate for the project's execution environment.

This project heavily uses DSharpPlus for Discord bot functionality, which is an asynchronous library.
Therefore, it is important to use async/await properly to avoid blocking the main thread and ensure responsiveness.

## DSharpPlus

[DSharpPlus](https://github.com/DSharpPlus/DSharpPlus) is a .NET library for interacting with the Discord API.
We are using the latest nightly versions of the library, which may include new features and bug fixes that are not yet available in the stable releases.

When using DSharpPlus, prefer using the latest features and APIs provided by the library.
Refer to their [Nightly releases documentation](https://dsharpplus.github.io/DSharpPlus/api/index.html) for guidance on how to use the new features and APIs.

## Logging

The project uses [serilog](https://github.com/serilog/serilog) for logging.
Prefer using the `ILogger` interface for logging, instead of writing to the console or using other logging frameworks.

Use structured logging with named properties to provide context for log messages.
Always use the appropriate log level for the message being logged (e.g., Information, Warning, Error).
Never log secrets, such as sensitive user info, tokens or API keys, in log messages.

## Collections

Collections have different performance characteristics and memory usage patterns.
Prefer using the appropriate collection based on the specific use case.
Prioritize choosing a collection with better performance and lower memory usage for the given scenario.
Do not just make everything a `List<T>` or `Dictionary<TKey, TValue>`, as it may not be the most efficient choice.

## Warnings

Avoid creating warnings in the code, such as values possibly being null.
All warnings should be resolved, not suppressed.

## LINQ

Use LINQ when it improves readability.
Avoid LINQ when an ordinary loop is significantly clearer or avoids unnecessary allocations.
Do not enumerate an `IEnumerable<T>` repeatedly when doing so causes unnecessary work.
Be aware of whether LINQ executes in memory or is translated by EF Core.

## Exception handling

Do not catch `Exception` unless there is a specific reason.
Do not catch exceptions merely to rethrow them without adding useful context or handling.
Preserve the original exception when wrapping exceptions.
Do not use exceptions for ordinary control flow.

## Appsettings

When using appsettings values, prefer using the `IOptions<T>` pattern for configuration.
The only exception is within Program.cs, where the `IConfiguration` interface can be used directly to read configuration values.

The types to use within `IOptions<T>` are located within the Config directory. These types are used to bind the appsettings values to strongly typed objects.

## Http & External APIs

Use `IHttpClientFactory` for outbound HTTP requests.
Do not instantiate `HttpClient` directly inside services.
Handle non-success HTTP responses explicitly.
Use DTOs for external API contracts.
Do not expose external API DTOs directly as internal domain models unless appropriate.

## Entity Framework

Use EF Core asynchronously for database operations.
Avoid unnecessary `ToListAsync()` calls when the query can remain composable.
Use `AsNoTracking()` for read-only queries where appropriate.
Avoid N+1 queries.
Do not perform database queries inside loops when the operation can be expressed as a single query.
Database migrations are source-controlled and should not be ignored.
Do not modify existing migrations that have already been applied to shared/production databases.

## Repositories

Repositories should encapsulate persistence concerns.
Do not place business logic in repositories.
Repositories should not contain Discord, HTTP, or presentation concerns.
Prefer returning domain/application models appropriate to the existing architecture rather than exposing EF Core internals unnecessarily.

## Services

Services contain application/business logic.
Services may coordinate repositories and other services.
Services should not directly manipulate HTTP responses.
Services should not contain presentation-specific concerns.

## Controllers/Commands & Endpoints

Controllers, commands, and endpoints should handle HTTP requests or Discord commands.
They should delegate business logic to services.
They should not contain complex business logic themselves.
They should return appropriate HTTP responses or Discord messages based on the outcome of the service calls.
They should only contain slight validation logic, such as checking for null or empty parameters, and should delegate more complex validation to services or validators.
Overall, controllers, commands, and endpoints should remain thin and focused on handling requests and responses.
Do not perform complex database operations directly in controllers/endpoints.
Return appropriate HTTP status codes.
Do not expose internal exception details.

## DTOs

Use DTOs for API contracts.
Do not expose EF Core entities directly from public API endpoints unless explicitly intended.
Request and response DTOs should represent the API contract rather than database structure.

## Validation

Validate external input at the application's boundary.
Do not rely exclusively on database constraints for user-facing validation.
Do not duplicate identical validation logic across multiple layers without a reason.

## Authentication & Authorization

Authentication and authorization must be enforced at the appropriate endpoint boundary.
Do not manually implement authentication checks in individual business methods when the ASP.NET Core authorization system can enforce them.
Never log authentication credentials or bearer tokens.
Never hardcode API keys or tokens.

## Background services

Background services must handle cancellation correctly.
Use the provided `CancellationToken`.
Do not allow an unhandled exception to unexpectedly terminate a long-running background process.
Avoid creating unmanaged background threads/tasks.

## Concurrency

Consider thread safety when modifying shared state.
Do not assume scoped services are safe to use concurrently.
Avoid shared mutable state unless there is a clear synchronization strategy.

## Performance

Avoid unnecessary allocations in frequently executed paths.
Avoid repeated enumeration of collections.
Avoid unnecessary materialization of LINQ queries.
Be conscious of database round trips.
Do not introduce caching without understanding cache invalidation and consistency requirements.

## Testing

The project uses xUnit for automated testing.
Tests must be repeatable and deterministic.
The test project should follow the same coding standards as the main project.
Remember to test edge cases and error conditions, not just the happy path.
Consider the SOLID principles when designing testable code.
This project prefers integration tests over unit tests and end-to-end tests, as it makes the code more maintainable and easier to refactor.

## Documentation & comments

Do not add comments that merely restate what the code does.
Prefer self-documenting code.
Use comments to explain why something is done, not what obvious code is doing.
Document public APIs when required by the project's conventions.

Comments should be for possible missing context, not for explaining the code itself.
The code should be clear enough to be understood without comments.

## Code style

Code style should prioritize readability, consistency, and maintainability.

The project's `.editorconfig` file is the authoritative source for formatting and analyzer rules. Follow the rules defined by `.editorconfig` rather than introducing separate formatting conventions.

Do not make formatting-only changes to unrelated code.

When modifying existing code, preserve the surrounding code style unless it conflicts with `.editorconfig` or there is a clear technical reason to change it.

## Anti-patterns

Avoid the following:

- Service locator patterns.
- `.Result` / `.Wait()`.
- Fire-and-forget tasks without explicit lifecycle management.
- Catching and swallowing exceptions.
- Hardcoded configuration.
- Hardcoded secrets.
- Direct `HttpClient` construction.
- N+1 database queries.
- Unnecessary abstractions.
- Giant service classes.
- Business logic in controllers.
- Database logic in controllers.
- Exposing EF entities as API contracts.
- Suppressing compiler warnings without justification.
- Disabling analyzers merely to make the build pass.

## Dependencies

Do not introduce a dependency when the .NET framework or an existing project dependency already provides the required functionality.
Before introducing a new dependency, consider whether the functionality can reasonably be implemented using existing dependencies.
