# GameNet Manager 4

Clean rebuild of the GameNet/CyberCafe management system.

## Architecture

- Native Windows Desktop is the operator surface.
- The Server is the single source of truth.
- The Client Agent is an execution/reporting boundary only.
- Desktop and Agent never connect directly to PostgreSQL.
- PostgreSQL is the production persistence store.
- Business code is organized as small feature modules and vertical slices.
- Financial, inventory and audit history use explicit domain rules and append-only records where history is authoritative.
- Shared code contains versioned contracts/primitives only.
- Production code has no mock/fake data source.

## Build order

1. Lean platform baseline
2. Station + Agent identity/health
3. Customer identity
4. Session start/use/end
5. Billing + wallet ledger
6. Transfer/release/concurrency
7. Discounts/VIP
8. Buffet/inventory
9. Reports/approvals/settings
10. Packaging, update and recovery hardening after the real product path works

Earlier projects became unstable by growing a large pre-feature foundation. Project 4 intentionally avoids that pattern.
