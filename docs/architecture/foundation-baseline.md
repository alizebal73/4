# GameNet Manager 4 — Foundation Baseline

Repository 4 is the clean implementation line for the Windows GameNet Manager product.

Repositories 2 and 3 are forensic references only. Code is not copied from them.

## Fixed architecture decisions

1. Native Windows Desktop is the only operator UI runtime.
2. Server is the only authority for business state, permissions, money, pricing, sessions and persistence.
3. Agent executes Server-authorized machine actions and reports device state.
4. Shared contains versioned transport contracts and small platform primitives only.
5. PostgreSQL is the intended authoritative production database.
6. Authoritative money uses integer Toman semantics.
7. Retryable mutations must define idempotency, concurrency and recovery before implementation.
8. External side effects belong after commit through a durable Outbox boundary.
9. DeviceId is device identity; IP address is connection metadata only.
10. Completion states are Designed, Implemented, Tested, Verified, Runtime Certified and Released.
11. No business vertical slice starts before Foundation certification.
12. scripts/verify.ps1 is the canonical local build/test gate; CI only orchestrates it.

## Construction order

Foundation
then Authentication/Permissions
then Station/Device
then Customer
then Session
then Billing/Wallet
then Tariff/VIP
then Buffet/Inventory
then Games/Accounts/Client Control
then Users/Shifts/Payroll/Cash
then Reservations/Maintenance/Network
then Reports/Notifications/Audit Explorer
then Backup/Update/Recovery/Release

## Bootstrap non-goals

- no fake dashboard data;
- no browser shadow application;
- no business logic in Program.cs, WPF code-behind or Agent transport;
- no startup migration as the production deployment strategy;
- no hard-coded production secrets;
- no installer before production provisioning and physical validation gates.
