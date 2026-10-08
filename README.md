# GameNet Manager 4

GameNet Manager 4 is the clean implementation line for a native Windows GameNet/CyberCafe management system.

Runtime boundaries:
- src/Server — authoritative Server/API boundary
- src/Desktop — native WPF Windows operator/admin application
- src/Client — Windows Agent on client PCs
- src/Shared — versioned contracts and small shared primitives
- tests — automated verification
- scripts/verify.ps1 — canonical local build/test gate

Repositories 2 and 3 are reference material, not code dependencies. Repository 4 starts from a clean platform skeleton so old implementation mistakes do not become structural dependencies.

Current status: Foundation Bootstrap.

The bootstrap currently contains the solution/project boundaries, a native Desktop shell, a Server health boundary, shared Money/EntityId primitives, automated tests and a corrected build workflow.

No business capability is marked complete yet.

Local verification on Windows with .NET SDK 10.0.100:

./scripts/verify.ps1

Next gate: Foundation Platform Certification for PostgreSQL/transactions, authentication/authorization, Agent transport/fencing, Desktop-to-Server smoke, recovery and release/update proof.
