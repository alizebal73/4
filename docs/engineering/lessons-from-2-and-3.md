# Forensic lessons and controls carried into 4

This document is the engineering memory of what went wrong in Repo 2 and what went wrong while correcting it in Repo 3.

The list is derived from the repositories' history, the recurring feature/fix sequences, Repo 2's bug-fix roadmap, and Repo 3's own comprehensive audit/closure documents. It is intentionally a compact control list, not a new certification bureaucracy.

## 1. What Repo 2 taught us

| ID | Historical failure pattern | Control in Repo 4 |
|---|---|---|
| R2-01 | Program.cs accumulated registration, endpoints, DTOs and business logic. | Program/Composition stays startup-only; feature behavior lives inside the owning module. |
| R2-02 | Data/large services became catch-all business containers. | No catch-all business folder; state and invariants have one feature owner. |
| R2-03 | Browser Dashboard became a second runtime with separate behavior and recurring JSX/TS repair. | Native WPF Desktop is the only operator runtime; no browser shadow product. |
| R2-04 | mockService.ts contained real-looking customers, finance, inventory and permissions. | No production fake source of truth. Fakes may exist only in explicitly isolated tests. |
| R2-05 | Server models, client models and mocks drifted (Station, buffet/inventory and other alignment fixes). | Shared transport contracts are explicit; each feature owns domain state; contract tests cover boundaries. |
| R2-06 | SQLite query behavior repeatedly differed from the intended production database. | PostgreSQL is the production store and concurrency/persistence verification must use PostgreSQL. |
| R2-07 | Migrations/model snapshots repeatedly drifted while features were moving. | Persistence changes are feature-owned in design, centralized only at the migration boundary, and verified before the slice closes. |
| R2-08 | Startup/deployment behavior became coupled to database initialization. | Production schema change belongs to the deployment/persistence boundary, not arbitrary feature startup code. |
| R2-09 | Customer identity, ClientKey, Agent identity and IP address were repeatedly confused. | Customer login identity and physical Agent DeviceId are separate concepts; IP is connection metadata only. |
| R2-10 | Multiple SignalR connections/reconnect paths could keep an Agent authoritative twice. | Connection lifecycle is explicit; one authoritative ownership/lease path; stale connections lose authority. |
| R2-11 | Session ownership was repaired late: station, Agent, customer login and lease could disagree. | Session ownership is one invariant across Station/Agent/Login/Session, enforced by the Server transaction. |
| R2-12 | Session Transfer initially moved StationId without atomically moving all related ownership. | Transfer is a single use case with atomic destination claim and complete ownership transition. |
| R2-13 | Dashboard and Agent could compute Session pricing/timing with different semantics. | Server owns tariff, billable time and settlement truth; clients submit intent and display returned state. |
| R2-14 | Wallet/payment/shift/report numbers drifted because different paths derived different revenue meanings. | Financial truth comes from authoritative ledgers and explicit financial semantics, not UI-derived totals. |
| R2-15 | Pending-payment and debt accounts could be confused by broad Draft-account queries. | Account state is explicit and every query/use case scopes the intended account state. |
| R2-16 | Settlement Preview could collect buffet entries from the wrong Session/account. | Preview is scoped to the selected authoritative Session and its own invoice/account linkage. |
| R2-17 | Warehouse and Showcase were treated as one stock pool. | Stock area is part of the invariant and every movement records its area/provenance. |
| R2-18 | Invoice reversal could restore stock to the wrong area. | Reversal is derived from the original movement/provenance, never from a generic default location. |
| R2-19 | Historical inventory cost and movement provenance had to be reconstructed later. | Authoritative movement records keep enough history to audit and reverse correctly. |
| R2-20 | Permissions were sometimes enforced in UI before the Server boundary was complete. | Authorization is Server-side; Desktop/Agent only reflect capability state. |
| R2-21 | Customer endpoints and Agent endpoints required later IDOR/ownership sweeps. | Every use case states its actor, target ownership and permission boundary before implementation. |
| R2-22 | Audit actor information was missing from some sensitive mutations. | Sensitive mutation contracts include actor/audit semantics as part of the use case. |
| R2-23 | Retry/reconnect/timeout behavior created duplicate or ambiguous outcomes. | Every retryable mutation has an explicit idempotency/uniqueness rule and transaction boundary when needed. |
| R2-24 | Time calculation was corrected repeatedly across session, VIP and dashboard paths. | One authoritative time source/business-time policy per Server feature; UI time is display-only. |
| R2-25 | Installer/update code became tightly coupled to changing internals and versions. | Packaging is a later boundary; it consumes stable Server/Desktop/Agent contracts rather than owning business logic. |
| R2-26 | CI/workflow scripts grew into a second program and generated excessive runs. | CI is intentionally small; it restores/builds/tests the product and does not become the product. |
| R2-27 | Physical/runtime problems were discovered after large amounts of feature work. | Important runtime behavior is proven at the end of each relevant vertical slice, not postponed until the very end. |

## 2. What Repo 3 taught us about over-correction

Repo 3 correctly identified most of the above failures, but then repeated a different engineering mistake: treating pre-feature Foundation completeness as the product itself.

| ID | Repo 3 over-correction | Rule for Repo 4 |
|---|---|---|
| R3-01 | Hundreds of commits were spent on Foundation, certification, closure, release and evidence before the first business slice. | Only build platform prerequisites that the next real slice actually needs. |
| R3-02 | Many documents became competing sources of truth before the product existed. | Keep a small number of canonical rules; do not create a new status document for every correction. |
| R3-03 | Guards and scripts became milestones in their own right. | A guard exists only to protect a concrete architectural/product invariant. |
| R3-04 | "Implemented" and "runtime proved" were carefully separated, but too much implementation was itself pre-feature hardening. | Distinguish structural readiness from runtime proof, while still moving quickly to the first real product path. |
| R3-05 | Security, release, SBOM, rollback, scale and recovery were pulled forward far beyond the first business need. | Introduce them at the feature boundary where they become necessary; deepen them before release, not before the first useful slice. |
| R3-06 | The closure matrix could block all business work while the product was still empty. | No global gate blocks the next slice unless its missing prerequisite can break that slice's correctness or safety. |
| R3-07 | Many foundation branches and status artifacts made repository state harder to reason about. | Keep one working line for the current product path; use short-lived branches/PRs for one coherent change. |
| R3-08 | Architecture was very well described, but real business invariants were still deferred. | A rule is not considered solved until the actual feature implementation and relevant test prove it. |

## 3. How Repo 4 applies the lessons

### Structural rules that exist now

- Server, Desktop and Agent are separate runtimes with explicit authority boundaries.
- Server feature modules own their state/invariants.
- Api -> Application -> Domain and feature-to-feature application contracts are the allowed paths.
- Shared contains transport contracts/primitives, not business truth.
- PostgreSQL is the production persistence target.
- No production mock/fake source.
- No browser operator runtime.
- No giant catch-all files or folders.
- CI is intentionally thin.

### Rules that must be proven when the feature is built

A slice is not complete because folders, DTOs or interfaces exist. The slice must prove the relevant invariant in the real implementation.

For a Station/Agent/Customer/Session slice, the minimum proof is:

1. Station and Agent identity are distinct and stable.
2. Only the Server can authorize session ownership and pricing.
3. Customer login is bound to the Server-resolved Agent identity.
4. A second/stale Agent connection cannot retain authority.
5. Session start claims the Station atomically.
6. Session end can only act on the owning Session/Login/Agent relationship.
7. Billable time and tariff come from Server state.
8. Restart/reconnect does not create duplicate ownership or duplicate settlement.
9. PostgreSQL integration tests cover the race-sensitive paths.
10. The Desktop only requests these operations and displays authoritative state.

### Rule for later slices

- Transfer/release must prove destination-claim atomicity and full ownership movement.
- Wallet/Billing must prove ledger correctness, retry/idempotency and finance/report reconciliation.
- Inventory/Buffet must prove Warehouse/Showcase provenance and reversal correctness.
- Permissions/Audit must prove Server-side authorization and actor attribution.
- VIP/discounts must prove one authoritative billing/time semantics.
- Packaging/update/recovery is added after the real product path exists and consumes stable contracts.

## 4. Historical evidence anchors

Representative history that exposed the above problems:

- Repo 2 be69b396: mock operations/reporting were added directly in the client source.
- Repo 2 1c8f628: even a Program declaration-order repair touched a very large Program.cs.
- Repo 2 acf3acf: Agent session pricing/login ownership had to be made Server-authoritative.
- Repo 2 2674e9e: Station claim had to be changed to an atomic database update.
- Repo 2 245417c: Agent connection confirmation/heartbeat/reconnect logic had to be hardened.
- Repo 2 81a452e: settlement preview had to be scoped to the selected Session.
- Repo 2 354bb4f: invoice reversal had to preserve historical inventory cost/provenance.
- Repo 2 550001a: the mock inventory model had to be repaired to match the real stock-area model.
- Repo 2 d803107: model/persistence alignment was still being repaired at installer time.
- Repo 2 docs/راه-اصلاح-باگ.md: the later forensic sweep explicitly recorded IDOR, Agent lifecycle, Transfer, account-state, VIP-time and Warehouse/Showcase bugs.
- Repo 3 docs/research/repo2-to-repo3-comprehensive-audit.md: these failures were converted into structural controls.
- Repo 3 docs/roadmap/00-pre-coding-roadmap.md and docs/architecture/foundation-closure-matrix.md: the corrective foundation then expanded far beyond the first business slice.

## 5. Working rule for 4

Build the smallest correct vertical slice.

Do not:
- solve every future release concern before the slice exists;
- create a second runtime to make the first runtime convenient;
- put business truth in UI, Agent, mocks or scripts;
- mark a feature complete because compilation or folder structure is green.

Do:
- name the owner of every invariant;
- make the Server authoritative where the business decision lives;
- make races explicit and test them against PostgreSQL;
- keep the change localized to the owning feature;
- prove the slice before starting the next one.

The objective of 4 is not "more foundation than 3". It is a product architecture where each building block is small, authoritative, testable and replaceable without destabilizing the rest of the system.
