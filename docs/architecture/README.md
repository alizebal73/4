# Architecture rules

This is the short operating contract for project 4.

## Ownership

**Server**
- Owns business state, authorization, pricing, sessions, billing, wallet, inventory and audit decisions.
- Owns concurrency and conflict resolution.
- Is the only component allowed to mutate production business data.

**Desktop**
- Displays server state and submits operator intents.
- Does not contain business truth.
- Does not connect to PostgreSQL.

**Agent**
- Identifies a physical device, reports health/state and executes server-authorized commands.
- Never calculates price, wallet balance, permission, session ownership or entitlement.
- Does not connect to PostgreSQL.

**Shared**
- Contains versioned DTOs/contracts and tiny transport primitives only.
- Never becomes a business-logic dumping ground.

## Module shape

Server features use:

Modules/<Feature>/Domain
Modules/<Feature>/Application
Modules/<Feature>/Infrastructure
Modules/<Feature>/Api

A feature can depend on another feature through an application contract. It cannot reach into another feature's EF entities or infrastructure.

## Data rules

- PostgreSQL is the production source of truth.
- Migration snapshots are generated from the actual EF model and are never hand-edited to silence drift.
- Money is integer Toman units or an explicit value object; floating point is forbidden for financial state.
- Ledger/history is authoritative for wallet, inventory movements and audit.
- Concurrency-sensitive commands require a database-safe invariant test.

## API rules

- Endpoints stay thin.
- Validation, authorization and business decisions belong in application/domain code.
- Errors use the versioned Shared V1 contract.
- Retriable mutating commands define an idempotency policy.
- Correlation IDs survive the whole request/command path.

## Testing rules

- No mocks/fakes in production assemblies.
- Unit tests prove domain invariants.
- Persistence tests use PostgreSQL for persistence rules.
- Every race bug gets a regression test proving the invariant.
