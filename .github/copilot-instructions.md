# Wealthify Copilot Instructions

## Project Context

- This repository is an ASP.NET Core Web API project targeting .NET 10.
- Data access uses Entity Framework Core with PostgreSQL.
- The codebase follows a layered structure:
    - Controllers -> Services -> Repositories -> DbContext

## Coding Conventions

- Use C# with nullable reference types in mind.
- Prefer async methods end-to-end and pass `CancellationToken` through controller, service, and repository layers.
- Keep controllers thin. Place business logic in services and data-access logic in repositories.
- Use DTOs for request/response contracts. Do not expose entity models directly from API endpoints.
- Keep naming clear and consistent with existing folders and interfaces (`I*Service`, `I*Repository`).

## API Conventions

- Use versioned route style consistent with the project, for example `/api/v1/...`.
- Return responses using existing envelope models:
    - `ApiResponse<T>` for data responses
    - `ApiResponse` for message/error-only responses
- For create endpoints, return `CreatedAtAction` where applicable.

## Error Handling

- Follow existing exception patterns in the `Exceptions` folder.
- Use global exception middleware for centralized handling instead of per-action try/catch blocks.
- For not-found flows, return a clear `ApiResponse` message and relevant error details.

## Dependency Registration

- Register new services and repositories in `Extensions/ServiceCollectionExtensions.cs`.
- Keep service and repository lifetimes aligned with current scoped registrations.

## Data Model Changes

- When adding a new entity:
    - Add the entity class under `Entity/`.
    - Update `ApplicationDbContext` with required `DbSet` and model configuration.
    - Create and apply EF Core migration.
    - Add repository, service, DTOs, and controller endpoints as needed.

## Working Style

- Make focused, minimal changes that align with existing project patterns.
- Preserve current architecture and API behavior unless explicitly asked to change it.
- When implementing new features, include all required layers (DTO, repository, service, controller, DI registration, migration if needed).

## User Preferences

- Ask clarifying questions first when any requirement is ambiguous. Do not assume missing details.
- Keep API route conventions explicit and stable. For `ExpenseType` use `/api/v1/expense-types` with:
    - `POST /api/v1/expense-types`
    - `GET /api/v1/expense-types`
    - `GET /api/v1/expense-types/{id}`
    - `PUT /api/v1/expense-types`
    - `DELETE /api/v1/expense-types/{id}`
