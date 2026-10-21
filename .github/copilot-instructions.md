# Kaffebar .NET API Instructions

This guide covers only `3-APIs/kaffebar-dotnet` and
`3-APIs/kaffebar-dotnet-solution`.

## Project boundaries

- `kaffebar-dotnet` is the .NET 10 workshop starter. It intentionally contains only a
  Minimal API, `GET /menu`, generated OpenAPI, and Scalar.
- `kaffebar-dotnet-solution` is the separate, runnable reference implementation for
  workshop tasks 1--10. Do not modify it when implementing a starter task unless the
  request explicitly targets the solution; use it to compare approaches.
- The API is code-first: C# endpoint and model definitions generate the OpenAPI
  contract. Treat `/openapi/v1.json` as a deliverable, not merely documentation.
  Scalar at `/scalar/v1` and `Kaffebar/Kaffebar.http` are the preferred ways to inspect
  endpoint behavior during development.

## Commands

The starter has no test project:

```bash
cd 3-APIs/kaffebar-dotnet/Kaffebar
dotnet run                    # API at http://localhost:5035
dotnet build
```

Run the Kaffebar .NET reference solution and its HTTP-level test suite from its directory:

```bash
cd 3-APIs/kaffebar-dotnet-solution/Kaffebar
dotnet run

cd ..
dotnet build
dotnet test
dotnet test --filter "FullyQualifiedName~KaffebarApiTests.CreateOrder_Returns201WithLocation"
```

## Architecture

- `Program.cs` owns application setup plus the intentionally small Minimal API surface:
  `GET /menu` and `GET /health`. Order operations belong in `Controllers/OrdersController`.
  Do not consolidate them into one style; this deliberate split demonstrates both
  Minimal APIs and controllers.
- `Models/` defines request/response records and enums. `Storage/` provides the
  in-memory `IOrderRepository` and deterministic seed data. `Errors/ProblemFactory`
  builds the API's RFC 7807 responses; reuse it rather than ad-hoc error payloads.
- `OpenApi/` patches gaps in the code-first generator (custom UTC timestamps, nullable
  enum schemas, omitted optional fields, and security metadata). Preserve these
  transformers whenever a change affects the wire contract.
- `Kaffebar.Tests` uses `WebApplicationFactory<Program>` and exercises HTTP routing,
  binding, validation, serialization, headers, and media types. Extend these
  integration tests for API changes; reset the singleton in-memory repository per test.
- Authentication and RabbitMQ events in the solution are optional and disabled by
  default. Keep public endpoints anonymous and do not enable either feature unless the
  request specifically requires it.

## Contract conventions

- Use distinct `record` types for requests and responses. Clients submit
  `CreateOrderRequest`; server-owned fields such as IDs, timestamps, status, and coffee
  names belong only to `Order`. Status changes use the dedicated
  `UpdateOrderStatusRequest`.
- Use `init`-property records for annotated request DTOs, not positional records:
  validation metadata must both run in MVC and appear correctly in generated OpenAPI.
  Use data annotations for validation and controller `[ApiController]` behavior for
  automatic `400` responses.
- Nullable reference types and default values are contract decisions: non-nullable
  properties become required; nullable optional properties are omitted from JSON; a
  default property value must also have `[DefaultValue]` when the OpenAPI contract needs
  to advertise it.
- Enums serialize as uppercase strings, never numeric values. Keep
  `JsonStringEnumConverter`, `JsonStringEnumMemberName`, strict numeric handling, null
  omission, and the UTC timestamp converter configured for **both** MVC
  (`AddJsonOptions`) and Minimal APIs (`ConfigureHttpJsonOptions`).
- Return resource-appropriate results and document them: `Created(...)` for a new
  order, `ProblemFactory` results for errors, and response metadata such as
  `.Produces<ProblemDetails>(...)` or `[ProducesResponseType]`. Verify errors remain
  `application/problem+json` with the contract's `Problem` schema name.
- After any endpoint, DTO, serializer, validation, auth, or error change, inspect
  `/openapi/v1.json` and Scalar. Confirm HTTP responses match the generated schema,
  especially required fields, enum values, timestamps, omitted optional fields, and
  problem responses.
