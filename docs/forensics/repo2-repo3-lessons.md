# Forensic Lessons Applied to Repository 4

## Repo 2 lessons

- The operator experience grew around a browser dashboard even though the target product is a Windows application.
- Large entrypoints and mixed responsibilities made changes risky.
- Preview scripts and CI became parallel execution paths.
- UI/mock paths appeared before authoritative Server paths.
- Physical 2–3 PC validation and production release gates remained incomplete.
- Production provisioning, LAN binding, service hosting, secret lifecycle and data-root separation were release blockers.
- Agent reconnect, session ownership and account allocation needed explicit server-side fencing and idempotency.

## Repo 3 improvements to keep

- native WPF Desktop;
- Server as the single source of truth;
- explicit Domain/Application/Infrastructure/API boundaries for business modules;
- versioned Shared contracts;
- PostgreSQL as authoritative persistence;
- transaction, idempotency, Outbox and connection-fencing concepts;
- durable Agent DeviceId separate from temporary ConnectionId;
- Server-side authorization;
- explicit Foundation certification;
- recovery and release evidence as first-class gates.

## Repo 4 rule

Keep the architectural lessons, but start from a small buildable skeleton. Do not copy code from either predecessor, do not create fake business success, and do not mark structural work as runtime-certified.
