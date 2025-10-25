# GEMINI Project Analysis: Duende.IdentityServer.Contrib.RedisStore

## Project Overview

This project is a .NET library that provides a Redis-based persistence layer for Duende IdentityServer. It serves as a store for operational data (`IPersistedGrantStore`) and a caching layer (`ICache<T>`), leveraging Redis for distributed storage and caching. The solution is structured into a main library project (`Duende.IdentityServer.Contrib.RedisStore`) and a corresponding test project (`Duende.IdentityServer.Contrib.RedisStore.Tests`). It targets .NET 6.0 and .NET 7.0.

The core logic revolves around storing and retrieving IdentityServer's persisted grants in Redis. To overcome Redis's non-relational nature, it cleverly stores multiple key entries for the same grant, enabling efficient lookups by subject ID, client ID, session ID, and type. It also utilizes Redis's key expiration features to manage the lifecycle of grants automatically.

### Key Features

*   **Redis-based Operational Store:** Persists operational data like authorization codes, reference tokens, and user consents to a Redis database.
*   **Distributed Caching:** Implements `ICache<T>` for caching frequently accessed data, reducing latency and database load.
*   **Flexible Configuration:** Provides extension methods for easy integration and configuration within the IdentityServer pipeline.
*   **Efficient Lookups:** Overcomes Redis's non-relational nature by storing multiple key entries for the same grant, enabling efficient lookups by subject ID, client ID, and session ID.

## Building and Running

The project uses the standard `dotnet` CLI for building and testing.

### Building the Project

To restore dependencies and build the solution in `Release` configuration:

```sh
dotnet restore
dotnet build --configuration Release --no-restore
```

### Running Tests

The project includes both unit and integration tests. A `docker-compose.yml` file is present to spin up a Redis instance for testing purposes.

To run all tests:

```sh
dotnet test
```

The `makefile` provides convenience targets for managing the test environment:

*   `make test`: Starts Redis via Docker, runs the tests, and stops the container.
*   `make up`: Starts the Redis container in the background.
*   `make down`: Stops the Redis container.

The CI pipeline, defined in `.github/workflows/tests.yml`, automates the build and test process on every pull request, running tests against .NET 6.0 and 7.0.

## Development Conventions

*   **Frameworks & Libraries:** The project is built using .NET with `Duende.IdentityServer` as the core dependency. `StackExchange.Redis` is used for Redis communication. For testing, it uses `XUnit`, `FluentAssertions`, and `Moq`.
*   **Configuration:** Service registration and configuration are handled via extension methods on `IIdentityServerBuilder` (e.g., `AddOperationalStore`, `AddRedisCaching`), which is a standard pattern in the IdentityServer ecosystem.
*   **Testing:** The `PersistedGrantStoreTests.cs` file demonstrates a clear testing strategy.
    *   Tests are written using the Arrange-Act-Assert pattern.
    *   `Moq` is used to mock dependencies like `ILogger` and `ISystemClock`.
    *   `FluentAssertions` is used for more readable and expressive assertions (e.g., `actual.Should().BeEquivalentTo(expected)`).
    *   Tests cover various scenarios, including storing, retrieving, removing grants, and handling expirations.
*   **Coding Style:** The C# code follows standard Microsoft conventions. It uses modern C# features like top-level statements, file-scoped namespaces, and asynchronous programming (`async`/`await`). Dependency injection is used throughout the library.

## Branching Strategy

This repository uses long-running branches to align with specific versions of the underlying Duende IdentityServer dependency. The naming convention for these branches is `releases/x.y.x`, where `x.y` corresponds to a major/minor version of Duende IdentityServer.

For example:
*   `releases/6.3.x` contains the code compatible with Duende IdentityServer `v6.3`.
*   `releases/5.2.x` contains the code compatible with Duende IdentityServer `v5.2`.

This approach ensures that maintenance and updates can be targeted to specific supported versions of Duende IdentityServer.

## Getting Started

To use this library in a Duende IdentityServer host project:

1.  Install the `StoveTech.Duende.IdentityServer.Contrib.RedisStore` NuGet package.
2.  In your `Startup.cs` or `Program.cs`, configure the `IIdentityServerBuilder` by calling the appropriate extension methods.

```csharp
// Example configuration
builder.AddIdentityServer()
    .AddOperationalStore(options =>
    {
        options.RedisConnectionString = "your_redis_connection_string";
        options.Db = 1;
    })
    .AddRedisCaching(options =>
    {
        options.RedisConnectionString = "your_redis_connection_string";
        options.Db = 2;
    });
```

For more detailed instructions, refer to the project's README and the official Duende IdentityServer documentation.

## Publishing

This package is published to NuGet. Different versions are maintained to correspond with different versions of Duende IdentityServer.

The package can be found at: [https://www.nuget.org/packages/StoveTech.Duende.IdentityServer.Contrib.RedisStore](https://www.nuget.org/packages/StoveTech.Duende.IdentityServer.Contrib.RedisStore)