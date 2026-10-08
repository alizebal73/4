# GameNet Manager 4

Windows desktop management system for a gaming center (گیم‌نت).

## Foundation

This repository starts from a controlled architecture rather than UI-only prototyping.

- .NET 10 solution and project structure
- WPF desktop shell
- Server API shell with live/readiness endpoints
- Agent executable boundary
- Domain/Application/Infrastructure/Contracts separation
- Integer-Toman Money primitive
- Clock abstraction
- PostgreSQL connection boundary
- Automated domain tests
- Product capability catalog and construction order

## Commands

    dotnet restore GameNet.slnx
    dotnet build GameNet.slnx --configuration Release
    dotnet test tests/GameNet.Domain.Tests/GameNet.Domain.Tests.csproj --configuration Release

The PostgreSQL provider is intentionally introduced in the persistence slice, not inside Domain or WPF.

See docs/product/capability-catalog.md for the complete product scope.
