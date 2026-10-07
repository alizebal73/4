# GameNet Manager 4 — Product Capability Catalog

This is the normalized product scope extracted from Repo 2 and Repo 3 history, product specifications, capability maps, completion backlogs, domain models, UI/navigation work, and the later forensic bug-fix records.

This is a product catalog, not a certification framework. A capability becomes "built" only when its real implementation, persistence/authority rules, permissions, relevant failure handling, tests, and operator workflow are actually complete.

## 1. Shop operation and Dashboard

### Dashboard / Control Center
- Station-centric live dashboard.
- PC, console/PS5 and foosball/table stations in one operational view.
- Card, compact/dense and list view modes.
- Grouping/filtering by station status.
- Grouping/filtering by VIP/normal.
- Grouping/filtering by Internet 1 / Internet 2.
- Grouping/filtering by remaining time.
- Station type filtering.
- Maintenance/offline filtering.
- Search.
- Multi-select with Ctrl/Shift/drag where batch operations are meaningful.
- Right-click station context menu.
- Quick operations without leaving the dashboard:
  - start session;
  - extend;
  - pause/resume;
  - change participants;
  - change tariff;
  - transfer;
  - end/settle;
  - wallet/debt operations;
  - buffet;
  - customer profile;
  - client control;
  - reservation/queue;
  - maintenance.
- "Needs attention" center.
- Persistent actionable attention items.
- Pending-payment follow-up.
- Session prepaid-credit remaining time and expiry warning.
- Manual session charge tracking by customer, station, amount and timestamp.
- Recent operator actions.
- Standard Persian error UX: what failed + meaning + next action.
- Notification center for actionable events.
- Server/LAN/Internet connectivity state kept distinct.
- Live Agent/device health reflected in station state.
- Server truth must win over local UI overrides.

### Station management
- Station CRUD.
- Station number/name.
- Generic station type.
- PC.
- PS5/console.
- Foosball/table.
- Zone/group.
- Active/disabled.
- Available/occupied.
- Reserved.
- Maintenance.
- Offline.
- Notes.
- Tariff assignment.
- Network profile/Internet assignment.
- Agent binding for PC-capable stations.
- Station capability metadata.
- Station health/last-seen.
- Customer/session summary.
- Maintenance/asset lifecycle.
- Atomic Agent-to-Station ownership rules.
- One active Station assignment per Agent device.

## 2. Operator identity, users, permissions and security

### Authentication
- Owner/Admin/Manager/Operator user accounts.
- Server-backed login/session.
- Logout/revocation.
- Current-user identity.
- Protected credentials.
- Session expiry.
- Role mapping.
- Customer authentication is a separate identity from operator authentication.

### Roles
- Owner.
- Manager/Admin.
- Operator.

### Permissions
- Fine-grained action permissions rather than page-only access.
- Read/write/execute/refund/reverse/settings/report/export style separation where needed.
- Server-side enforcement.
- Permission-aware Desktop UI.
- Permission scopes for sensitive reports/actions when required.
- Operator cannot bypass financial/security rules by manipulating UI requests.

### Sensitive-operation approval
- Approval request.
- Requester.
- Approver.
- Separation of duties.
- Approved/rejected/cancelled states.
- Reason/decision note.
- Expiry where needed.
- Execution linked to the approved operation.
- High discount approval.
- Refund approval.
- Invoice reversal approval.
- Payroll/employee financial approvals.
- Sensitive inventory/destructive adjustments where policy requires.

### Section protection
- Optional per-section PIN/lock UX.
- Must never replace Server authorization.
- Protected sections include examples such as Users/Shift, Customers, Accounts, Games, Reports, Tariffs, Buffet, Settings and Client control.

## 3. Customers

### Customer identity/profile
- Customer code/ID.
- Full name.
- Mobile/contact.
- National ID where required.
- Username/alias where required.
- Customer PIN/credential.
- Status/active state.
- Notes.
- Search/filter.
- Customer master/detail UI.

### Customer financial state
- Wallet balance.
- Debt.
- Free Money.
- Free Time.
- VIP status.
- VIP tier.
- VIP package.
- Package remaining time/allowance.
- Daily usage.
- Transaction/history timeline.

### Customer actions
- Create.
- Edit.
- Activate/deactivate.
- Charge wallet.
- Deduct wallet.
- Record debt.
- Pay debt.
- Add Free Money.
- Consume Free Money.
- Add Free Time.
- Consume Free Time.
- Refund.
- Reverse eligible financial operations.
- Add/read notes.
- Open current sessions.
- View history.

### Customer login/session identity
- Customer login tied to the Server-resolved Agent/device.
- Active login tracking.
- Concurrent login limit per customer.
- Login acquire/release.
- Logout.
- No trust in arbitrary client-supplied device keys.
- Customer A must never be able to query or mutate Customer B data.

## 4. Stations, Agent and client machine control

### Agent identity
- Durable DeviceId.
- Separate device identity from temporary ConnectionId.
- Secure bootstrap/provisioning.
- Credential lifecycle.
- Credential rotation/revocation.
- Protected local credential storage.
- Device-to-Station binding.
- Device-to-Station uniqueness.

### Agent connectivity
- Authenticated SignalR transport.
- Heartbeat.
- Last-seen.
- Online/offline/stale states.
- Reconnect.
- Connection fencing.
- One authoritative lease/connection per DeviceId.
- Stale connection loses authority.
- Server restart/reconnect handling.
- State reconciliation.
- Dashboard Agent health.

### Server-authorized Client commands
- Lock.
- Unlock.
- Logout.
- Logout + lock.
- Restart.
- Shutdown.
- Start game.
- Stop game.
- Maintenance mode.
- Display/message command where policy permits.
- Network/Internet selection command where integration permits.
- Diagnostics.
- Update.
- Rollback.
- Command timeout.
- Persisted command state.
- Acknowledgement/result.
- Retry-safe behavior.
- Duplicate-command protection.
- Pending command failure when Agent becomes stale.

### Kiosk / shell
- Fullscreen GameNet lock screen.
- Disconnect safety policy.
- Lock on disconnect if enabled.
- Kiosk policy.
- Safe logout.
- Controlled app/game access.
- Recovery after Agent restart.

### Client telemetry
- CPU/system/process status when needed.
- Running game/process observation.
- Effective lock state.
- Heartbeat state.
- Diagnostics/last error.
- Version/build.
- Update state.
- Maintenance state.

## 5. Sessions

### Session lifecycle
- Reservation.
- Start.
- Active.
- Pause.
- Resume.
- Extend.
- Reduce.
- Change participant count.
- Change tariff according to policy.
- Transfer.
- End.
- Settlement.
- Payment.
- Debt handoff.
- Receipt.
- Cancel/recovery where applicable.

### Session rules
- PC is one person.
- PS5/console can support multiple participants subject to tariff/policy.
- Server-authoritative tariff.
- Server-authoritative billable time.
- Session pricing snapshot.
- Optional controlled manual rate override for the operator.
- Prepaid amount and calculated end time.
- Postpaid session has no fake remaining time.
- Pause and time adjustment are part of authoritative billing time.
- Session owns Customer + Station + Agent + CustomerLogin relationships.
- Session ownership remains consistent during transfer.
- Transfer atomically claims destination Station.
- Transfer moves all relevant ownership/lease/login relationships.
- Session end requires ownership validation.
- Reconnect/restart does not create duplicate Session or settlement.

### Session Center
- Operational details.
- Timeline.
- Customer.
- Station.
- Agent state where relevant.
- Participants.
- Tariff.
- Manual rate.
- Prepaid/remaining state.
- Pause/resume.
- Time adjustment.
- Buffet totals.
- Payment/settlement actions.
- Reversible operations where supported.

## 6. Tariffs and pricing

### Tariff management
- Create.
- Edit.
- Activate/deactivate/archive.
- Station type.
- Normal/VIP policy.
- Hourly rate.
- Daily rate.
- Minimum duration.
- Time unit.
- Rounding.
- Time bands/night rates.
- Weekend/holiday policy if enabled.
- Participant policy.
- Overflow policy.
- Effective date/time policy.

### Pricing integrity
- Server owns pricing.
- Historical Session retains the applicable pricing snapshot.
- Future tariff edits cannot rewrite settled historical Sessions.
- Customer/Agent cannot submit a price that becomes authoritative.
- Operator manual price changes are permission-controlled.
- High discount requires configured policy/approval.
- Discount ceiling enforced Server-side.
- Explainable settlement breakdown.

## 7. VIP

### VIP plans
- Plan/package catalog.
- Title/tier.
- Price.
- Duration.
- Daily time.
- Total time.
- Discount percentage.
- Eligible station/tariff scope.
- Buffet discount/perks.
- Overflow rule.
- Active/archived.

### Customer VIP
- Assign package.
- Start/end.
- Remaining days.
- Remaining daily allowance.
- Daily consumption.
- Usage history.
- Package expiry.
- VIP-aware pricing.
- VIP-aware settlement.
- VIP-aware reports.

### VIP integrity
- Consumption uses the same authoritative billable-time semantics as Session billing.
- Pause/time adjustment/cancel cannot create inconsistent VIP consumption.
- Package expiry is enforced Server-side.

## 8. Wallet, money, billing and settlement

### Wallet ledger
- Top-up/charge.
- Debit.
- Credit.
- Settlement.
- Refund.
- Reverse.
- Source/reference links.
- Immutable history.
- Balance derived from authoritative ledger semantics.

### Money rules
- Canonical unit: Toman.
- Authoritative amounts use integer money semantics / explicit Money primitive, not floating point.
- Wallet and Cash Register are separate.
- Free Money and Free Time are separate ledgers/benefits.
- Historical references are retained.

### Payment
- Cash.
- Card/POS.
- Transfer.
- Wallet.
- Combined/split payment where enabled.
- External provider-ready model; provider integration is separate from core accounting.

### Settlement
- Session cost.
- Buffet cost.
- Discount.
- VIP benefit.
- Free Money.
- Free Time.
- Prepaid amount.
- Paid amount.
- Remaining due.
- Wallet effect.
- Debt effect.
- Breakdown/"Why this amount?" view.
- Payment allocation to the correct Session/Invoice.
- Reversal/refund semantics.

### Refund/reversal
- Amount.
- Reason.
- Source/reference.
- Permission.
- Approval when required.
- Audit.
- No deletion/editing of historical financial truth.
- Idempotent/retry-safe behavior.

## 9. Cash register, shifts and staff

### Cash register
- Opening cash.
- Cash sales.
- Card/POS sales.
- Transfers.
- Wallet-related external cash effects.
- Refunds.
- Expenses.
- Manual adjustments with reason.
- Expected closing cash.
- Actual closing cash.
- Difference.
- Reconciliation.
- Handover.

### Shifts
- Start shift.
- Start cash.
- Active shift.
- Close shift.
- Cash reconciliation.
- Sales linked to operator.
- Refunds/discounts/expenses linked to actor and time.
- Handover notes.
- Difference is not silently converted into employee debt.

### Staff profile
- Identity/contact.
- Role.
- Permissions.
- Hourly/monthly pay type.
- Hourly rate.
- Monthly salary.
- Overtime rate.
- Employment start date.
- Work schedule.
- Active/inactive.

### Payroll ledger
- Calculated salary.
- Paid salary.
- Remaining payable.
- Salary advance.
- Bonus.
- Deduction.
- Damage/shortage records.
- Owner receivable vs employee payable kept separate.
- Manual adjustment with reason.
- Approval.
- Payment receipt.
- Payment method.
- Audit.

## 10. Buffet

### Product catalog
- Name.
- Category.
- Unit.
- Sale price.
- Buy/cost price.
- Active/inactive.
- Product code/barcode where needed.
- Minimum stock.
- Maximum/order level.
- Current stock.
- Warehouse stock.
- Showcase stock.

### Sales
- Quick sale.
- Sale to active Session.
- Sale to Customer account.
- Standalone sale.
- Cart.
- Quantity.
- Payment.
- Wallet.
- Debt.
- Receipt.
- Daily sales.

### Buffet/session integration
- Sale belongs to the selected authoritative Session.
- Draft invoice belongs to the intended account state.
- Session settlement includes the correct buffet items.
- Existing invoice items are not silently mutated during settlement.
- Buffet entries are visible in Session timeline/history.

## 11. Inventory

### Stock movements
- Purchase/in.
- Sale/out.
- Adjustment.
- Waste.
- Return.
- Stock count/reconciliation.
- Warehouse → Showcase transfer.
- Future stock-location transfer support.

### Inventory history
- Quantity.
- Unit price.
- Unit cost.
- Stock area.
- Movement kind.
- Source invoice.
- Operator.
- Timestamp.
- Notes/provenance.

### Inventory controls
- Atomic stock decrement.
- Insufficient-stock conflict.
- Concurrent mutation protection.
- Minimum-stock warning.
- Zero-stock warning.
- Purchase-needed list.
- Historical cost retained.
- Reverse/return restores the original stock-area/provenance correctly.
- Warehouse is not substituted for Showcase in customer-facing availability.

### Suppliers/purchasing
- Supplier.
- Purchase record.
- Supplier invoice.
- Purchase price.
- Payable.
- Payment.
- Purchasing history.
- Inventory impact.

## 12. Games

### Game catalog
- Name.
- Version.
- Genre/category.
- Launcher/platform.
- Executable/App ID.
- Install path.
- Launch arguments.
- Target OS/system.
- Target station type/zone.
- Compatibility.
- Active/disabled.
- Cover.
- Optional trailer.
- Status.
- Search.
- Filters/categories.
- Master/detail editing.

### Game execution
- Server-authorized launch.
- Stop.
- Running process detection.
- Session-bound launch validation.
- Reject launch on stale/non-owning client.
- Game/Station compatibility checks.
- Real Agent execution and acknowledgement.

## 13. Game accounts / License Pool

### Pool
- Launcher/platform grouping.
- Account catalog.
- Login identity metadata.
- Provider.
- Game association.
- Availability state.
- Active/inactive.
- Allocation.

### Lease
- Atomic allocation.
- Current station/session.
- Lease acquire/release.
- Release reason.
- History.
- Concurrency protection.
- Health/state.
- No double allocation.

### Security
- Secrets protected.
- No raw secret leakage into ordinary UI.
- Server authority over allocation.
- Agent only receives an authorized execution payload if needed.

## 14. Reservations and waiting queue

### Reservation
- Customer.
- Station or station type.
- Start/end.
- Tariff.
- Status.
- Confirm.
- Check-in.
- Complete.
- Cancel.
- No-show.
- Extension.
- Notes.

### Queue
- Customer.
- Requested station type.
- Join timestamp.
- Priority/policy.
- Notification.
- Assignment.
- Cancellation.
- Live Dashboard integration.

## 15. Maintenance and assets

### Maintenance
- Reason.
- Severity.
- Reported by.
- Assigned technician.
- Parts.
- Cost.
- Status.
- Start/end.
- Return to service.
- Maintenance notes.

### Assets
- PC.
- Console.
- Controller.
- TV/display.
- Headset.
- Keyboard/mouse.
- Printer.
- POS.
- Router.
- Switch.
- Access Point.
- Server.

## 16. Network

### Network profiles
- Internet 1.
- Internet 2.
- Gateway/profile.
- DNS/policy.
- Station assignment.
- Exceptions.
- Health/diagnostics.

### Dashboard
- Group/filter PCs by Internet 1 / Internet 2.
- Explicit visual separation.
- Preserve Agent Online/Offline state.
- Network state is authoritative Server/Agent information.
- Router automation remains an integration boundary.

### Operational requirement
The system must support shops using multiple ISPs without confusing network grouping with station ownership.

## 17. Reports

### Financial
- Revenue.
- Payments.
- Wallet activity.
- Debt.
- Discounts.
- Free Money.
- Free Time.
- Refunds.
- Reversals.
- Expenses.
- Profit.

### Sessions/stations
- Session count.
- Occupancy.
- Usage duration.
- Station revenue.
- PC/console/table utilization.
- Filters by station/type/operator/time.

### Customers/VIP
- Customer activity.
- New customers.
- Wallet activity.
- Debt.
- VIP usage.
- Package consumption.
- Expiry/renewal.

### Buffet/inventory
- Product sales.
- Best sellers.
- Product profit/margin.
- Current stock.
- Low stock.
- Waste.
- Returns.
- Purchases.

### Users/shifts/payroll
- Operator sales.
- Shift totals.
- Cash reconciliation.
- Difference.
- Expenses.
- Payroll activity.
- Audit actor.

### Audit
- Sensitive operations.
- Who did it.
- When.
- What changed.
- Result.
- Reason.
- Correlation/operation id.

### Reporting UX
- Report Center.
- Today / Yesterday / 7 days / 30 days / 6 months / Year / Custom.
- Contextual filters.
- Summary + detail table.
- CSV/Excel/PDF only where permission/requirement permits.
- No report becomes a second accounting authority.
- Reports consume authoritative ledgers/read models.

## 18. Notifications and attention

- Critical/warning/info levels.
- Read/unread.
- Timestamp.
- Source.
- Target.
- Action/deep link.
- Deduplication.
- Retry semantics.

Examples:
- Agent offline.
- Session ending.
- Pending payment.
- Debt.
- Low stock.
- Backup failure.
- Health failure.
- Failed client command.
- Update failure.

## 19. Audit and approvals

### Audit
- Actor.
- Operation.
- Target.
- Before/after where appropriate.
- Timestamp.
- Correlation/operation id.
- Result.
- Reason.
- Append-only semantics.

### Approval
- Action.
- Requester.
- Approver.
- Separation of duties.
- Decision.
- Reason.
- Expiry.
- Executed-operation link.

## 20. Settings

### Groups
- General/GameNet.
- Dashboard/display.
- Localization/time/currency.
- Session/settlement/rounding.
- Tariffs/VIP.
- Billing/payment.
- Alerts/sound/popup.
- Stations.
- Network/Internet 1-2.
- Client policy/WOL/Agent.
- Games/Game Accounts.
- Buffet/Inventory.
- Users/Roles/Permissions.
- Shifts/Payroll.
- Security.
- Backup/Recovery.
- Printing/Receipts.
- Updates/Release.
- Notifications.
- Keyboard/Hotkeys.
- Appearance/Accessibility.
- Advanced/Diagnostics.

### Setting semantics
Every operational setting must have:
- owner;
- type;
- default;
- validation;
- scope;
- authorized roles/actions;
- effective time;
- persistence location;
- audit requirement;
- restart requirement;
- local/server authority.

## 21. Backup and recovery

- Manual backup.
- Scheduled backup.
- Backup list/history.
- Verification.
- Restore preparation.
- Restore.
- Retention.
- Destination.
- Failure status.
- Recovery alerts.
- Recovery audit.
- PostgreSQL dump/restore for production.
- Isolated restore validation.

## 22. Installation, update and rollback

- Server installation.
- Agent installation.
- Desktop installation.
- Arbitrary installation paths.
- DataRoot separate from install directory.
- Windows Service registration.
- Version manifest.
- Compatibility.
- Checksums.
- Signing.
- Side-by-side staging.
- Health verification.
- Update.
- Rollback.
- Local incremental update path.
- Later cloud/update transport as an extension.
- Component boundaries remain independent.

## 23. Printing

- Printer selection.
- Thermal/A4.
- Paper size.
- Copies.
- Auto print.
- Preview.
- GameNet header/logo/contact.
- Receipt numbering.
- Session receipt.
- Payment receipt.
- Buffet receipt.
- Shift reports.

## 24. Diagnostics / support

- Server health.
- PostgreSQL health.
- API health.
- SignalR health.
- Agent health.
- Station health.
- Backup health.
- Update health.
- Network health.
- Version/build.
- Last seen.
- Last error.
- Connectivity test.
- Diagnostic/support information without secrets.

## 25. Desktop UX system

- Native WPF operator shell.
- RTL-first fa-IR.
- LTR en-US.
- Dense operator mode.
- Normal mode.
- Keyboard-first workflows.
- Search/command palette.
- F1 contextual help where useful.
- Right-click Station/Client actions.
- Controlled drag/drop where it improves a defined task.
- Loading/empty/error/offline/stale/permission/conflict/retry states.
- Shared visual primitives:
  - Summary Card;
  - Entity Card;
  - Info Panel;
  - Data Table/List;
  - Status Pill;
  - Badge;
  - Modal;
  - Drawer;
  - Toast;
  - Empty/Error/Offline/Loading states.
- Numeric/Money/Duration/Search inputs.
- 1366x768 usable.
- No unnecessary horizontal overflow.
- Localization/terminology consistency.
- Accessibility and keyboard semantics.

## 26. Product-wide non-negotiable rules

1. Server is the authoritative source of truth.
2. Desktop is an operator client, not a business engine.
3. Agent executes/reports; it never decides authorization, pricing, customer ownership or settlement.
4. PostgreSQL is production persistence.
5. Money is canonical Toman and authoritative amounts are integer/explicit-money semantics.
6. Wallet and Cash Register are different domains.
7. Free Money and Free Time are distinct.
8. DeviceId is device identity; IP is connection metadata.
9. Agent ownership uses explicit connection/lease fencing.
10. Session ownership is consistent across Customer + Login + Station + Agent.
11. Transfer is atomic.
12. Inventory preserves movement provenance and stock area.
13. Reversal never deletes financial/inventory history.
14. Sensitive mutations require Server permission and audit.
15. Retryable mutations have explicit idempotency/concurrency rules.
16. Grouping/filtering is presentation only.
17. No production mock/fake source of truth.
18. No browser shadow runtime.
19. Each vertical slice is built, tested and verified before the next slice.
20. A capability is not complete because its screen exists.

## 27. Recommended construction order

### Slice 0 — platform prerequisites
- PostgreSQL connection/persistence boundary.
- Versioned Shared contracts.
- Money primitive.
- Time abstraction point.
- Thin health/readiness.
- Real test project structure.

### Slice 1 — operator + station/device base
- Operator authentication.
- Roles/permissions.
- Station CRUD.
- Agent registration/pairing.
- Durable DeviceId.
- Agent heartbeat/reconnect/fencing.
- Station ↔ Agent binding.
- Dashboard live station/Agent state.

### Slice 2 — customer
- Customer CRUD/profile.
- PIN/identity.
- Customer login.
- Concurrent login limit.
- Customer ↔ Agent binding.

### Slice 3 — session
- Start/end.
- Pause/resume.
- Extend/reduce.
- Participant rules.
- Server tariff resolution.
- Server billable-time authority.
- Session Center.
- Agent-driven session path.

### Slice 4 — billing/wallet
- Invoice.
- Wallet ledger.
- Payment methods.
- Split settlement.
- Debt.
- Free Time/Free Money.
- Refund/reverse.
- Approval.
- Audit.

### Slice 5 — transfer/concurrency
- Atomic station claim.
- Session transfer.
- Agent/login/lease ownership movement.
- Concurrent operations tests.

### Slice 6 — tariff/VIP/discount
- Tariff management.
- VIP packages.
- Usage/entitlement.
- Operator discount ceilings.
- High-discount approval.

### Slice 7 — buffet/inventory
- Product catalog.
- Warehouse/Showcase.
- Purchases.
- Sale.
- Session/customer/standalone sale.
- Waste/return/adjustment.
- Profit/stock reports.

### Slice 8 — games/accounts/client control
- Game catalog.
- Account pools.
- Lease allocation.
- Real launch/stop.
- Lock/unlock.
- Kiosk policy.
- Power control.
- Process detection.

### Slice 9 — users/shift/payroll/cash
- Staff profiles.
- Shifts.
- Cash register.
- Payroll.
- Employee ledger.
- Approvals.

### Slice 10 — reservations/queue/maintenance/network
- Reservation.
- Waitlist.
- Maintenance/assets.
- Internet 1/2 profiles and assignment.
- Diagnostics.

### Slice 11 — reports/notifications/audit explorer
- Report Center.
- Filters/date ranges.
- Audit Explorer.
- Notification center.
- Export where authorized.

### Slice 12 — backup/update/recovery/release
- Backup/restore.
- Installation.
- Update.
- Rollback.
- Recovery.
- Signing/manifest/SBOM.
- Final real-machine validation.

## 28. Completion rule

The complete product scope is represented here. Work may be delivered incrementally, but no capability should be marked complete until its authoritative rule, real persistence path, contracts, permission/audit behavior where needed, relevant concurrency/retry/recovery behavior, tests and operator workflow agree with the product contract.
