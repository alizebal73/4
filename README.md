# GameNet Manager 4

Clean rebuild of the GameNet/CyberCafe management system.

## Product scope

The complete extracted capability catalog is maintained in:

docs/product/capability-catalog.md

It is the normalized product scope from Repo 2 and Repo 3. It covers shop operations, Dashboard, Stations, Customers, Agent/Client control, Sessions, Tariffs, VIP, Wallet/Billing, Cash/Shift/Payroll, Buffet/Inventory, Games/Game Accounts, Reservations/Queue, Maintenance, Network, Reports, Notifications, Audit/Approvals, Settings, Backup/Recovery, Printing, Diagnostics and Update/Rollback.

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

1. Lean platform prerequisites
2. Operator authentication/permissions + Station + Agent identity/health
3. Customer identity and concurrent login
4. Session start/use/end and Session Center
5. Billing + wallet ledger + payment/debt
6. Transfer/release/concurrency
7. Discounts/VIP
8. Buffet/inventory
9. Games/game accounts/client control
10. Users/shift/payroll/cash
11. Reservations/queue/maintenance/network
12. Reports/notifications/audit explorer
13. Packaging, update, backup, recovery and release hardening after the real product path works

Earlier projects became unstable in two different ways: Repo 2 grew business complexity without strong enough boundaries, while Repo 3 grew the pre-feature foundation far beyond the first real product slice. Project 4 deliberately avoids both patterns.

A platform prerequisite is added only when it is needed to make the next real slice correct, testable or safe.