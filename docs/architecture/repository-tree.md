# GameNet Manager 4 — Repository Tree

The repository is intentionally organized before business implementation starts.

## Runtime tree

src/
- Server/
  - Host/                 composition root and process startup only
  - Api/                  transport endpoints and API adapters
  - Application/          use-case orchestration and application ports
  - Domain/               domain primitives/rules that belong to the platform
  - Infrastructure/       database, messaging and external implementations
  - Modules/              business modules; each module owns a complete vertical boundary
- Desktop/
  - Shell/                WPF application shell and windows
  - Features/             operator-facing feature presentation
  - Application/          Desktop-side application services
  - Infrastructure/       HTTP, configuration and local platform integration
  - Localization/         fa-IR and en-US resources
  - Assets/               icons and visual assets
- Client/
  - Host/                 Agent process startup/composition
  - Core/                 device identity and local state foundations
  - Transport/            Server communication
  - Features/             server-authorized machine capabilities
  - Infrastructure/       Windows and local OS integration
- Shared/
  - Contracts/V1/         versioned transport contracts only
  - Primitives/           small cross-runtime primitives only

tests/
- Shared.Tests/
- Server.Tests/
- Desktop.Tests/
- Agent.Tests/
- Contract.Tests/
- Integration.Tests/
- E2E/

## Business module rule

A business module lives under Server/Modules/<Module>/ with:
- Domain
- Application
- Infrastructure
- Api
- Contracts when a cross-boundary contract is required

A module may not reach into another module's persistence classes.

## Foundation rule

Platform code is separate from business modules.

Business modules are added only after their Definition of Ready is complete. We do not create dozens of placeholder business folders merely to make the tree look finished.

## Ownership rule

Server is authoritative. Desktop and Agent are clients of Server authority.

## File responsibility rule

Each source file has one primary reason to change. Composition roots contain wiring, not business behavior. UI files contain presentation, not business rules. Transport files contain transport concerns, not domain rules.
