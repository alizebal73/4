# GameNet Manager 4

GameNet 4 is the clean-slate desktop GameNet manager rebuilt from the product capability catalog extracted from the previous projects.

## Architecture

- **Server**: modular-monolith authority and business rules.
- **Desktop**: native WPF operator application; it never owns business truth.
- **Agent**: separate Windows worker responsible for client-machine execution/reporting.
- **PostgreSQL**: production persistence boundary.
- **Contracts**: API contracts shared by clients.
- **Domain**: business invariants and value objects.
- **Application**: use-case boundaries.
- **Infrastructure**: PostgreSQL and external adapters.

The first commit is **Slice 0 — platform foundation**. It intentionally contains no fake business data and no browser-shadow runtime.

## Local prerequisites

- .NET SDK 10.0.401 or a compatible 10.0 feature band.
- PostgreSQL 17+.
- Windows for the WPF Desktop and Windows Agent.

Set the connection string before starting the server:

PowerShell:

    $env:ConnectionStrings__PostgreSQL="Host=127.0.0.1;Port=5432;Database=gamenet;Username=gamenet;Password=CHANGE_ME"

Run the server:

    dotnet run --project src/Server

The health endpoint is:

    http://127.0.0.1:5080/api/health

Run the desktop:

    dotnet run --project src/Desktop

The Desktop shell currently proves the real server/database connection. Operator authentication, stations, agents, sessions and billing are built as separate vertical slices after this foundation.

## Engineering rule

A capability is only considered complete when its authoritative rule, persistence path, permissions, audit/retry behavior where applicable, tests and operator workflow agree.
