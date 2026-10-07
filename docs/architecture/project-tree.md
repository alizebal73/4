# Project 4 tree

The tree is a boundary map, not a place to hide complexity.

## Server

- `Composition/`: startup and module registration only.
- `Infrastructure/`: cross-cutting technical concerns only.
- `Persistence/`: DbContext and generated migrations only.
- `Modules/<Feature>/Domain`: business invariants and domain state owned by that feature.
- `Modules/<Feature>/Application`: use cases and public application contracts.
- `Modules/<Feature>/Infrastructure`: feature-specific persistence/integration adapters.
- `Modules/<Feature>/Api`: thin HTTP/realtime entry points.

A module never reaches into another module's Domain or Infrastructure. Cross-module behavior goes through application contracts.

## Desktop

Desktop is an operator client, not a second server.

- `Api/`: server transport client.
- `Shell/`: application shell.
- `UI/`: shared navigation, commands and UI state.
- `Features/<Feature>/Views`
- `Features/<Feature>/ViewModels`
- `Infrastructure/`: desktop-only hosting/configuration.

No Desktop feature owns business truth or pricing logic.

## Agent

- `Identity/`: physical device identity and credential storage.
- `Transport/`: authenticated Server connection.
- `Capabilities/`: server-authorized device capabilities.
- `Hosting/`: Windows Service lifetime.
- `Infrastructure/`: local technical adapters.

The Agent never decides authorization, customer ownership, session ownership or money.

## Shared

Only stable transport contracts and tiny primitives belong here. Shared is not a common business layer.

## Tests

- `Server.UnitTests`: domain/application behavior.
- `Server.IntegrationTests`: real persistence and HTTP behavior.
- `Agent.Tests`: identity, transport and capability behavior.
- `Desktop.Tests`: view models and client behavior.
- `ContractTests`: versioned protocol/API compatibility.
- `E2E`: real end-to-end product paths.

## Hard rules learned from 2 and 3

1. No catch-all `Data`, `Services`, `Utils` or `Helpers` folders for business code.
2. No giant Program/controller/service/page files.
3. A feature owns its state and invariants.
4. UI/API code submits intents; it does not mutate entities directly.
5. PostgreSQL is the production persistence target.
6. Tests for concurrency/persistence must exercise PostgreSQL.
7. No production mocks/fakes.
8. Cross-cutting infrastructure is added only when a real feature needs it.
9. Release/update/recovery tooling stays outside the product path until its stage requires it.
10. One vertical slice at a time.
