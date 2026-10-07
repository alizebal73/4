# Lessons carried into 4

These are constraints, not another certification bureaucracy.

1. No giant Program/controller/service/view files.
2. No split authority: Server decides ownership, sessions, permissions and money.
3. No direct entity edits from UI/API glue; state changes go through use cases/domain rules.
4. No SQLite production path; PostgreSQL is the real persistence target.
5. No production mock/fake source of truth.
6. Customer identity and physical device identity are explicit protocol concepts.
7. Transfer/session/wallet races are solved with transactions and invariants, not timing.
8. Agent connection and reconnect lifecycle are explicit; stale clients cannot keep authority.
9. Inventory movement is auditable and reversible without corrupting stock history.
10. Desktop screens stay feature-oriented and composed from small view models.
11. CI remains small and verifies the product rather than becoming a second codebase.
12. Release/update/SBOM/recovery hardening is staged after the real product path exists.
13. Each feature slice is implemented, tested and verified before the next slice starts.
