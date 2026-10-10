# .NET Clean Architecture Template

[![CI](https://github.com/engMohammedHussein/dotnet-clean-architecture-template/actions/workflows/ci.yml/badge.svg)](https://github.com/engMohammedHussein/dotnet-clean-architecture-template/actions/workflows/ci.yml)
[![MIT license](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

A reference ASP.NET Core 8 Minimal API demonstrating **Clean Architecture**, **CQRS with MediatR**, **FluentValidation**, and **EF Core** — the same patterns used in production backend services for the Saudi market (ZATCA e-invoicing integrations, KYC/digital-identity services, and internal business APIs).

Built by **Mohammed Daghmah** — Senior Mobile & SDK Architect specializing in Flutter, .NET Core, and secure digital identity systems. This is the backend counterpart to [`flutter_clean_architecture_template`](https://github.com/engMohammedHussein/flutter_clean_architecture_template) — same architectural discipline, different side of the wire.

## Why this template exists

Most ".NET Clean Architecture" boilerplate on GitHub stops at folder names and an empty `WeatherForecast` endpoint. This one implements a complete, working vertical slice — a **Products** feature — through all four layers: domain entity with real invariants, CQRS commands/queries with validation, EF Core persistence, and Minimal API endpoints, wired end to end with dependency injection and a global exception-to-ProblemDetails mapper, so you can see exactly how a request flows in and a response flows back out.

It runs out of the box with **no external database** — demo mode uses EF Core's InMemory provider, seeded with sample data on startup. Setting a `ConnectionStrings:Default` value switches to a real SQLite file on disk, and nothing in the Application or Api layers changes. That swap is the whole point of the architecture — the same idea as the Flutter template's fake-vs-real data source toggle.

## Architecture

```
src/
├── Domain/                      # Enterprise business rules — no dependencies on anything
│   ├── Entities/                 # Product: a rich entity, not an anemic DTO
│   ├── Interfaces/               # IProductRepository — the contract Infrastructure implements
│   ├── Exceptions/               # DomainException, NotFoundException, DuplicateSkuException
│   └── Common/                   # BaseEntity, Result<T>
│
├── Application/                  # Use cases, CQRS-style
│   ├── Products/
│   │   ├── Commands/              # CreateProduct, UpdateProduct, DeleteProduct
│   │   ├── Queries/                # GetProducts, GetProductById
│   │   ├── DTOs/                   # ProductDto — the public contract, separate from the entity
│   │   └── Mappings/               # Entity → DTO, hand-written
│   └── Common/
│       ├── Behaviors/              # ValidationBehavior — a MediatR pipeline behavior
│       └── Exceptions/             # ValidationException
│
├── Infrastructure/                # Implementation details
│   └── Persistence/
│       ├── ApplicationDbContext.cs
│       ├── Configurations/         # EF Core fluent configuration (Fluent API, not attributes)
│       ├── Repositories/           # ProductRepository — the only class that knows EF Core exists
│       └── Seed/                   # Demo data
│
└── Api/                           # Minimal API host
    ├── Endpoints/                  # ProductEndpoints — route → MediatR.Send() → Results
    ├── Middleware/                 # ExceptionHandlingMiddleware → RFC 7807 ProblemDetails
    └── Program.cs

tests/
├── Domain.UnitTests/              # Entity invariants, in isolation
├── Application.UnitTests/         # Command/query handlers against a fake in-memory repository
└── Api.IntegrationTests/          # Full HTTP pipeline via WebApplicationFactory
```

## The dependency rule

Dependencies always point inward:

```
Api  →  Infrastructure  →  Application  →  Domain
Api  ───────────────────→  Application
```

`Domain` references nothing. `Application` references only `Domain`. `Infrastructure` implements `Application`'s and `Domain`'s interfaces but is never referenced *by* them. `Api` is the only layer allowed to know both Application and Infrastructure exist, because it's the composition root. This is what makes the domain and application logic testable without a database, and swappable — EF Core could be replaced with Dapper or MongoDB by rewriting `Infrastructure/Persistence/Repositories/ProductRepository.cs` alone.

## Key patterns demonstrated

| Pattern | Where |
|---|---|
| Rich domain entity with private setters and invariants enforced in-method | `Domain/Entities/Product.cs` |
| Repository pattern (interface in Domain, implementation in Infrastructure) | `Domain/Interfaces/IProductRepository.cs` / `Infrastructure/.../ProductRepository.cs` |
| CQRS with MediatR (one command/query, one handler) | `Application/Products/Commands`, `Application/Products/Queries` |
| Pipeline-based validation (FluentValidation + MediatR behavior) | `Application/Common/Behaviors/ValidationBehavior.cs` |
| Centralized exception → HTTP mapping (ProblemDetails, RFC 7807) | `Api/Middleware/ExceptionHandlingMiddleware.cs` |
| Minimal API endpoints grouped by feature, no controllers | `Api/Endpoints/ProductEndpoints.cs` |
| Swappable persistence (InMemory demo ↔ SQLite) behind one DI switch | `Infrastructure/DependencyInjection.cs` |
| Fake-repository testing (no mocking library needed for most handler tests) | `tests/Application.UnitTests/Common/InMemoryProductRepository.cs` |
| Full-pipeline integration tests via `WebApplicationFactory` | `tests/Api.IntegrationTests/` |

## Getting started

```bash
dotnet restore
dotnet run --project src/Api
```

Then open `http://localhost:5080/swagger` — the API is pre-seeded with five sample products, so every endpoint is immediately explorable.

Run the tests:

```bash
dotnet test
```

### Switching to a real database

In `src/Api/appsettings.json` (or an environment variable), set:

```json
{ "ConnectionStrings": { "Default": "Data Source=app.db" } }
```

`Infrastructure/DependencyInjection.cs` picks this up automatically and switches from the InMemory provider to SQLite — no other code changes.

## Testing strategy

| Layer | What is tested | How |
|---|---|---|
| Domain | Entity invariants — can't go negative stock, can't have an empty name, etc. | Plain xUnit + FluentAssertions, no mocks |
| Application | Handlers delegate correctly, branch on found/not-found/duplicate | A hand-written in-memory fake repository (same contract as the real one) |
| Api | Full request → validation → handler → persistence → response round-trip | `WebApplicationFactory<Program>` against an isolated InMemory database per test class |

## Adapting this to a new feature

1. Add your entity under `Domain/Entities/`, with a private constructor and a `Create` factory enforcing its invariants.
2. Define its repository contract in `Domain/Interfaces/`.
3. Add a folder under `Application/<Feature>/` with `Commands/`, `Queries/`, `DTOs/`, and a mapping extension.
4. Implement the repository in `Infrastructure/Persistence/Repositories/`, with an `IEntityTypeConfiguration<T>` under `Configurations/`.
5. Register the repository in `Infrastructure/DependencyInjection.cs`.
6. Add a `<Feature>Endpoints.cs` under `Api/Endpoints/` and map it in `Program.cs`.
7. Mirror the test structure: entity tests in `Domain.UnitTests`, handler tests in `Application.UnitTests`, endpoint tests in `Api.IntegrationTests`.

## License

MIT — use freely as a starting point for your own projects.
