# GameNet Manager — Foundation Architecture

## Goal

Build the Windows desktop operator application as one client of a server-authoritative system. Business rules, authorization, pricing, billing, inventory and session truth must not live in the WPF UI.

## Components

- GameNet.Desktop: WPF operator application, RTL-first for fa-IR.
- GameNet.Server.Api: authoritative HTTP boundary.
- GameNet.Agent: Windows station agent; executes only server-authorized commands.
- GameNet.Application: use-case orchestration and ports.
- GameNet.Domain: business primitives and invariants without UI/database/network dependencies.
- GameNet.Infrastructure: PostgreSQL and external-system adapters.
- GameNet.Contracts: transport DTOs shared across process boundaries.
- GameNet.Domain.Tests: automated domain tests.

## Non-negotiable rules

1. Server is the source of truth.
2. Desktop is an operator client, not a business engine.
3. Agent executes and reports; it does not authorize or calculate pricing.
4. PostgreSQL is the production persistence target.
5. Money is integer Toman; floating-point money is forbidden.
6. DeviceId is device identity; IP/ConnectionId are connection metadata.
7. Mutations need explicit authorization, audit and retry/concurrency semantics before completion.
8. Financial and inventory history is append-oriented; reversal creates compensating records.
9. Each vertical slice must compile, test and verify before it is treated as complete.

Slice 0 establishes the platform boundary and build/test skeleton. Slice 1 starts operator authentication, roles/permissions, station CRUD, durable Agent identity, pairing, heartbeat/fencing and live dashboard state.

See docs/product/capability-catalog.md for the complete product scope.
