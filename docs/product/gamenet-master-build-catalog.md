# GameNet Manager 4 — Master Build Catalog

> سند مادر محصول، معماری، نقشه ساخت، مهارت‌های مهندسی، Promptهای اجرایی و معیارهای تکمیل.
>
> این سند مرجع اصلی ساخت GameNet 4 است. هر قابلیت فقط وقتی «ساخته‌شده» محسوب می‌شود که مسیر واقعی Domain → Application → Persistence → API/Transport → Desktop/Agent، تست، مجوز، خطا، همزمانی و Recovery آن کامل باشد.

---

## 0. قانون حاکم بر سند

این سند از سه منبع اصلی ساخته شده است:

1. نیاز واقعی محصول GameNet 98.
2. قابلیت‌ها و باگ‌های استخراج‌شده از Repo 2.
3. درس‌های معماری و پایگاه Repo 3.

هر تغییر آینده باید با این سند سازگار باشد.

### قواعد غیرقابل مذاکره

1. Server مرجع حقیقت و تصمیم‌گیری است.
2. Desktop فقط Operator Client است.
3. Agent فقط اجرا، گزارش، Heartbeat و Recovery را انجام می‌دهد.
4. Desktop و Agent مستقیماً به PostgreSQL وصل نمی‌شوند.
5. PostgreSQL منبع تولیدی سیستم است.
6. پول با واحد Toman و primitive صریح Money/Integer نگهداری می‌شود.
7. Wallet، Cash Register، Debt، Free Money و Free Time حوزه‌های مجزا هستند.
8. DeviceId هویت پایدار Agent است؛ ConnectionId فقط اتصال موقت است.
9. Agent ownership با Lease/Fencing کنترل می‌شود.
10. Session مالکیت Customer + CustomerLogin + Station + Agent را سازگار نگه می‌دارد.
11. Transfer باید Atomic باشد.
12. Inventory provenance و Stock Area از بین نمی‌روند.
13. Financial/Inventory history حذف یا overwrite نمی‌شود؛ Reverse/Compensate انجام می‌شود.
14. عملیات حساس Server-side permission و Audit دارد.
15. عملیات قابل Retry باید Idempotency/Concurrency مشخص داشته باشند.
16. UI منبع حقیقت نیست.
17. Mock/Fake/Shadow Runtime در production ممنوع است.
18. Screen ساخته‌شدن به معنی Capability Complete نیست.
19. هر Slice باید Build + Test + Persistence + Workflow واقعی داشته باشد.
20. تا Slice فعلی Verified نشده، Slice بعدی شروع نمی‌شود.

---

# 1. محصولی که قرار است بسازیم

## 1.1 هویت محصول

**GameNet Manager 4** یک سیستم کامل مدیریت CyberCafe/GameNet برای Windows است که عملیات روزانه فروشگاه را مدیریت می‌کند:

- PC
- PS5 / Console
- Foosball / Table
- Customer
- Session
- Billing
- Wallet
- Debt
- VIP
- Tariff
- Buffet
- Inventory
- Games
- Game Accounts
- Agent/Client Control
- Operators
- Roles/Permissions
- Shift
- Cash Register
- Payroll
- Reservation
- Queue
- Maintenance
- Network / ISP
- Reports
- Audit
- Approvals
- Backup/Recovery
- Update/Rollback
- Diagnostics

محصول باید یک **Operator Desktop Native** داشته باشد، نه یک Web UI که در مرورگر نقش برنامه اصلی را بازی کند.

---

# 2. Vision نهایی

## 2.1 Runtime Map

```text
                     ┌──────────────────────────┐
                     │        WPF Desktop       │
                     │     Operator Client      │
                     └────────────┬─────────────┘
                                  │
                             HTTP / SignalR
                                  │
                                  ▼
                     ┌──────────────────────────┐
                     │     ASP.NET Core Server   │
                     │  Business Authority       │
                     │  Auth / Use Cases / Rules │
                     └────────────┬─────────────┘
                                  │
                           EF Core / Npgsql
                                  │
                                  ▼
                     ┌──────────────────────────┐
                     │       PostgreSQL         │
                     │ Production Source Truth  │
                     └──────────────────────────┘

       ┌──────────────────────────┐
       │ Windows Agent             │
       │ PC-side execution/report  │
       └────────────┬─────────────┘
                    │
              Authenticated
              Agent Transport
                    │
                    ▼
              Server Authority
```

## 2.2 Responsibility Map

| Component | مسئولیت | چه چیزی نباید انجام دهد |
|---|---|---|
| Domain | قواعد، invariants، entity/value object | HTTP، EF details، UI |
| Application | Use Case و orchestration | rendering/UI |
| Infrastructure | DB، Security، external adapters | business UI |
| Server | HTTP/SignalR composition + auth enforcement | operator screen |
| Desktop | نمایش و command UX | تصمیم قیمت/حسابداری |
| Agent | اجرای فرمان + heartbeat + telemetry | pricing/auth/settlement |
| PostgreSQL | persistence authoritative | business workflow مستقل |

---

# 3. Product Map

## 3.1 نقشه حوزه‌ها

```text
GameNet
│
├── Operator / Security
│   ├── Login
│   ├── Roles
│   ├── Permissions
│   ├── Sessions
│   └── Approvals
│
├── Shop Operations
│   ├── Dashboard
│   ├── Stations
│   ├── Customers
│   ├── Sessions
│   ├── Reservations
│   └── Queue
│
├── Billing
│   ├── Tariffs
│   ├── VIP
│   ├── Wallet
│   ├── Debt
│   ├── Free Money
│   ├── Free Time
│   ├── Payments
│   ├── Settlement
│   └── Refund / Reverse
│
├── Commerce
│   ├── Buffet
│   ├── Products
│   ├── Purchases
│   ├── Inventory
│   ├── Warehouse
│   └── Showcase
│
├── Client Control
│   ├── Agent
│   ├── Device
│   ├── Lease
│   ├── Lock / Unlock
│   ├── Game Launch
│   └── Diagnostics
│
├── Resources
│   ├── Games
│   ├── Game Accounts
│   ├── Assets
│   ├── Maintenance
│   └── Network Profiles
│
├── Staff
│   ├── Shifts
│   ├── Cash Register
│   ├── Employees
│   └── Payroll
│
├── Intelligence
│   ├── Reports
│   ├── Notifications
│   ├── Audit
│   └── Attention Center
│
└── Platform
    ├── Backup
    ├── Restore
    ├── Update
    ├── Rollback
    ├── Deployment
    └── Diagnostics
```

---

# 4. Desktop UX Map

## 4.1 Shell

```text
Main Window
│
├── Top Toolbar
│   ├── Current Operator
│   ├── Server State
│   ├── Notifications
│   ├── Search
│   └── Help / F1
│
├── Navigation
│   ├── Dashboard
│   ├── Stations
│   ├── Customers
│   ├── Sessions
│   ├── Billing
│   ├── Buffet
│   ├── Inventory
│   ├── Games
│   ├── Reports
│   ├── Users / Shifts
│   └── Settings
│
└── Work Area
    └── Feature Page
```

## 4.2 Dashboard

Dashboard باید سریع‌ترین مسیر انجام کار Operator باشد.

ویژگی‌ها:

- Station-centric live view
- PC / PS5 / Foosball
- Card/List/Dense
- Filter
- Search
- Status grouping
- VIP/Normal
- ISP 1 / ISP 2
- Remaining Time
- Maintenance
- Offline
- Needs Attention
- Right-click context menu
- Multi-select
- Quick Actions

### Context Menu

- Start Session
- Extend
- Pause
- Resume
- Change Customer
- Change Tariff
- Transfer
- End / Settle
- Wallet
- Debt
- Buffet
- Customer Profile
- Client Control
- Reservation
- Maintenance

---

# 5. Master Capability Catalog

## 5.1 Dashboard / Shop Operation

- Live station dashboard
- PC/Console/Foosball
- Card/List/Dense views
- Filter/search/group
- VIP/Normal
- ISP1/ISP2
- Remaining Time
- Maintenance/Offline
- Needs Attention
- Pending Payment
- Expiry warnings
- Manual charge history
- Notification center
- Recent operator actions
- Server/LAN/Internet states
- Agent health

## 5.2 Station

- CRUD
- Number/Name
- Station Type
- Zone/Group
- Enable/Disable
- Maintenance
- Occupied/Available/Reserved
- Tariff assignment
- Network profile
- Agent binding
- Capabilities
- Health
- Last seen
- Customer/session summary
- Asset lifecycle

## 5.3 Operator / Security

### Authentication

- Owner
- Manager/Admin
- Operator
- Login
- Logout
- Expiry
- Revocation
- Current identity
- Separate customer identity
- Secure password storage

### Permissions

- Fine-grained permissions
- Read/Write
- Execute
- Refund
- Reverse
- Settings
- Reports
- Export
- Client Control
- Server-side enforcement
- Permission-aware UI

### Approvals

- Request
- Approver
- Separation of duties
- Pending
- Approved
- Rejected
- Cancelled
- Expiry
- Reason
- Linked execution

## 5.4 Customers

### Profile

- Customer ID
- Full Name
- Mobile
- National ID
- Alias
- PIN
- Status
- Notes
- Search

### Financial State

- Wallet
- Debt
- Free Money
- Free Time
- VIP
- Package
- Daily usage
- History

### Actions

- Create/Edit
- Activate/Deactivate
- Charge Wallet
- Debit Wallet
- Record Debt
- Pay Debt
- Add/Consume Free Money
- Add/Consume Free Time
- Refund
- Reverse
- Notes
- Current Sessions

### Identity Rules

- Server-resolved customer login
- Concurrent login limit
- Acquire/release
- Customer isolation

## 5.5 Agent / Client

### Identity

- Durable DeviceId
- Provisioning
- Pairing
- Credential lifecycle
- Rotation
- Revocation
- Secure local storage
- Device ↔ Station uniqueness

### Connectivity

- SignalR
- Authenticated transport
- Heartbeat
- Last Seen
- Online
- Stale
- Reconnect
- Lease/Fencing
- Server restart reconciliation

### Commands

- Lock
- Unlock
- Logout
- Logout + Lock
- Restart
- Shutdown
- Start Game
- Stop Game
- Maintenance
- Message
- Network select
- Diagnostics
- Update
- Rollback

### Command Integrity

- Persisted command state
- Ack/Result
- Timeout
- Retry
- Duplicate protection
- Failure on stale Agent

### Telemetry

- CPU
- Processes
- Game process
- Effective lock
- Heartbeat
- Error
- Version
- Update state
- Maintenance

## 5.6 Sessions

Lifecycle:

```text
Reservation
   ↓
Start
   ↓
Active
 ┌─┴──────┐
Pause   Extend/Adjust
 ↓
Resume
 ↓
Transfer
 ↓
End
 ↓
Settlement
 ↓
Payment / Debt
```

Rules:

- PC = one person
- Console = configurable participants
- Server tariff
- Billable time Server authority
- Pricing snapshot
- Pause/resume
- Adjustments
- No fake remaining time
- Customer/Station/Agent/Login consistency
- Atomic Transfer
- No duplicate settlement
- Recovery after restart

## 5.7 Tariffs

- Create
- Edit
- Activate
- Archive
- Station type
- Normal/VIP
- Hourly
- Daily
- Minimum duration
- Unit
- Rounding
- Time bands
- Weekend/Holiday
- Participant policy
- Overflow
- Effective date

Rules:

- Historical pricing snapshot
- Future tariff doesn't mutate history
- Client cannot choose authoritative price
- Manual override permission
- Discount ceiling
- Approval

## 5.8 VIP

- Plans
- Tier
- Price
- Duration
- Daily time
- Total time
- Discount
- Station scope
- Buffet perks
- Expiry
- Remaining time
- Consumption history
- Renewal
- VIP pricing

## 5.9 Wallet / Billing

### Wallet Ledger

- Credit
- Debit
- Settlement
- Refund
- Reverse
- Source
- Reference
- Immutable history

### Payments

- Cash
- POS/Card
- Transfer
- Wallet
- Split payment

### Settlement

- Session
- Buffet
- Discount
- VIP benefit
- Free Money
- Free Time
- Prepaid
- Paid
- Due
- Wallet effect
- Debt effect
- Breakdown

### Refund

- Amount
- Reason
- Permission
- Approval
- Audit
- Idempotency
- No deletion

## 5.10 Cash Register / Shift / Payroll

### Cash Register

- Opening cash
- Cash sales
- POS
- Transfers
- Refund
- Expenses
- Adjustment
- Expected closing
- Actual closing
- Difference
- Reconciliation
- Handover

### Shift

- Start
- Opening cash
- Active
- Close
- Reconciliation
- Operator linkage
- Difference handling

### Employee

- Profile
- Role
- Permissions
- Pay type
- Hourly
- Monthly
- Overtime
- Start date
- Schedule

### Payroll

- Calculated salary
- Paid
- Remaining
- Advance
- Bonus
- Deduction
- Damage/shortage
- Approval
- Payment
- Employee ledger

## 5.11 Buffet

### Catalog

- Name
- Category
- Unit
- Sale price
- Buy price
- Code/barcode
- Min stock
- Max/order level
- Active

### Sales

- Quick sale
- Session sale
- Customer sale
- Standalone
- Cart
- Quantity
- Payment
- Wallet
- Debt
- Receipt

### Integration

- Correct Session ownership
- Correct account
- Correct settlement linkage
- Timeline/history

## 5.12 Inventory

- Purchase
- Sale
- Adjustment
- Waste
- Return
- Count
- Warehouse → Showcase
- Provenance

### Inventory Integrity

- Atomic decrement
- Insufficient stock conflict
- Concurrency protection
- Min stock
- Zero stock
- Purchase-needed
- Historical cost
- Reverse with original area/provenance

### Purchasing

- Supplier
- Purchase
- Supplier invoice
- Cost
- Payable
- Payment
- History

## 5.13 Games

- Game catalog
- Name
- Version
- Genre
- Launcher/platform
- Executable/AppId
- Path
- Arguments
- Station compatibility
- Cover
- Trailer
- Active/Disabled
- Search/filter

### Execution

- Server-authorized launch
- Stop
- Process detection
- Session binding
- Agent acknowledgment
- Stale-agent rejection

## 5.14 Game Account Pool

- Account catalog
- Platform
- Provider
- Game
- Availability
- Allocation
- Lease
- Release
- History
- Concurrency protection
- Secret protection

## 5.15 Reservation / Queue

### Reservation

- Customer
- Station/type
- Start/end
- Tariff
- Confirm
- Check-in
- Complete
- Cancel
- No-show
- Extension

### Queue

- Customer
- Requested type
- Timestamp
- Priority
- Notification
- Assignment
- Cancellation

## 5.16 Maintenance / Assets

- Ticket
- Reason
- Severity
- Reporter
- Technician
- Parts
- Cost
- Status
- Start/End
- Return to service
- Notes

Assets:

- PC
- Console
- Controller
- TV
- Headset
- Keyboard/Mouse
- Printer
- POS
- Router
- Switch
- AP
- Server

## 5.17 Network

- Internet 1
- Internet 2
- Gateway
- DNS/policy
- Station assignment
- Exception
- Health
- Diagnostics

Rule:

**ISP grouping is presentation/integration state; it is never Station ownership.**

## 5.18 Reports

Financial:

- Revenue
- Payments
- Wallet
- Debt
- Discounts
- Free Money
- Free Time
- Refund
- Reverse
- Expenses
- Profit

Session:

- Count
- Occupancy
- Duration
- Station revenue
- Type utilization

Customer/VIP:

- Activity
- Wallet
- Debt
- VIP usage
- Expiry
- Renewal

Buffet/Inventory:

- Sales
- Best sellers
- Margin
- Stock
- Low stock
- Waste
- Returns
- Purchases

Staff:

- Operator totals
- Shift
- Cash
- Difference
- Expenses
- Payroll

Audit:

- Actor
- Operation
- Target
- Time
- Result
- Reason
- Correlation id

## 5.19 Notifications

- Critical
- Warning
- Info
- Read/unread
- Timestamp
- Source
- Target
- Action link
- Deduplication
- Retry

Examples:

- Agent Offline
- Session Ending
- Payment Pending
- Debt
- Low Stock
- Backup Failure
- Client Command Failure
- Update Failure

## 5.20 Audit / Approvals

### Audit

- Actor
- Operation
- Target
- Before
- After
- Timestamp
- Correlation/Operation ID
- Result
- Reason
- Append-only

### Approval

- Requester
- Approver
- Action
- Decision
- Reason
- Expiry
- Linked operation

## 5.21 Settings

Groups:

- General
- Dashboard
- Localization
- Currency
- Session
- Rounding
- Tariff
- VIP
- Billing
- Alert
- Station
- Network
- Client
- Games
- Buffet
- Inventory
- Users
- Roles
- Permissions
- Shift
- Payroll
- Security
- Backup
- Printing
- Updates
- Notifications
- Hotkeys
- Accessibility
- Diagnostics

Every setting has:

- Owner
- Type
- Default
- Validation
- Scope
- Authorized action
- Effective time
- Persistence location
- Audit requirement
- Restart requirement
- Authority

## 5.22 Backup / Recovery

- Manual backup
- Scheduled backup
- History
- Verification
- Restore
- Retention
- Destination
- Failure state
- Alert
- Audit
- PostgreSQL dump/restore
- Isolated restore validation

## 5.23 Install / Update / Rollback

- Server installer
- Desktop installer
- Agent installer
- Arbitrary install path
- DataRoot separate from install dir
- Windows Service
- Version manifest
- Compatibility
- Checksums
- Signing
- Staging
- Health verification
- Update
- Rollback
- Local update path

## 5.24 Printing

- Printer selection
- Thermal/A4
- Paper size
- Copies
- Auto print
- Preview
- Logo/header
- Receipt number
- Session receipt
- Payment receipt
- Buffet receipt
- Shift report

## 5.25 Diagnostics

- Server health
- DB health
- API
- SignalR
- Agent
- Station
- Backup
- Update
- Network
- Version/build
- Last error
- Connectivity test
- Support package without secrets

---

# 6. Domain Map

## 6.1 Core aggregate candidates

```text
Identity
├── Operator
├── OperatorSession
├── Role/Permission
└── Approval

Operations
├── Station
├── AgentDevice
├── Customer
├── CustomerLogin
└── Session

Commercial
├── Tariff
├── VIPPlan
├── CustomerVIP
├── WalletLedger
├── Invoice
├── Payment
└── Refund/Reversal

Commerce
├── Product
├── Sale
├── InventoryMovement
├── StockLocation
├── Supplier
└── Purchase

Client Control
├── AgentLease
├── ClientCommand
├── ClientTelemetry
└── GameProcess

Staff
├── Shift
├── CashRegister
├── Employee
└── PayrollLedger

Planning
├── Reservation
└── QueueEntry

Support
├── MaintenanceTicket
├── Asset
├── AuditRecord
└── Notification
```

---

# 7. Dependency Map

```text
Contracts
   ↑
Application
   ↑
Domain

Infrastructure → Domain/Application
Server → Contracts/Application/Infrastructure
Desktop → Contracts
Agent → Contracts

PostgreSQL ← Infrastructure only

Desktop ──HTTP/SignalR──► Server
Agent   ──SignalR/API──► Server
```

### قانون

هیچ مسیر مجازی دیگری ایجاد نمی‌شود:

- Desktop → DB ❌
- Agent → DB ❌
- Desktop → Domain persistence internals ❌
- Agent → business authority ❌
- Browser Shadow Runtime ❌

---

# 8. Map ساخت Vertical Slice

هر Slice این مسیر را طی می‌کند:

```text
Product Rule
   ↓
Domain Invariants
   ↓
Application Use Case
   ↓
Contract
   ↓
Persistence
   ↓
Server Endpoint / Transport
   ↓
Desktop / Agent Workflow
   ↓
Unit + Integration + Contract Tests
   ↓
Real Database Verification
   ↓
CI
   ↓
Real-machine validation when required
```

---

# 9. Build Plan

## Slice 0 — Platform Foundation

وضعیت: Foundation ایجاد شده و CI سبز شده است.

شامل:

- .NET 10
- WPF
- ASP.NET Core
- PostgreSQL/Npgsql
- EF Core
- Shared Contracts
- Money
- IClock
- Health
- Base test project
- GitHub Actions

### Gate 0

- Restore ✅
- Build ✅
- Test ✅
- Desktop project exists ✅
- Server starts ✅
- PostgreSQL boundary exists ✅
- No mock source ✅

---

# 10. Slice 1 — Operator + Station + Agent

## Scope

### Operator

- User/Password
- Login
- Session
- Roles
- Permission
- Logout/revocation

### Station

- CRUD
- Number uniqueness
- Type
- Lifecycle
- Binding

### Agent

- Durable DeviceId
- Registration
- Pairing code
- Credential
- Secure token
- Heartbeat
- ConnectionId
- LeaseVersion
- Stale
- Reconnect
- Station binding

### Desktop

- Login Page
- Dashboard shell
- Station list
- Agent health
- Binding UI
- Permission-aware commands

### Gate 1

- PostgreSQL schema/migration ✅
- Login uses real DB ✅
- Invalid password rejected ✅
- Expired session rejected ✅
- Permission enforced Server-side ✅
- Station number unique ✅
- Agent DeviceId unique ✅
- Pairing code expires ✅
- Old Agent connection cannot heartbeat after fencing ✅
- One agent ↔ one station rule ✅
- Desktop displays real state ✅
- Tests green ✅

---

# 11. Slice 2 — Customer

- Customer CRUD
- Profile
- PIN
- Customer login
- Active login tracking
- Concurrent login limit
- Customer ↔ Agent binding

Gate:

- Customer A cannot access B
- Login identity is Server-derived
- Duplicate login policy works
- Real DB tests pass

---

# 12. Slice 3 — Session

- Start
- End
- Pause
- Resume
- Extend
- Reduce
- Participants
- Tariff resolution
- Billable time
- Session Center
- Agent flow

Gate:

- No duplicate active session
- Station ownership consistent
- Customer ownership consistent
- Agent ownership consistent
- Restart/reconnect recovery
- Real settlement-ready data

---

# 13. Slice 4 — Billing / Wallet

- Invoice
- Wallet ledger
- Payment
- Split payment
- Debt
- Free Money
- Free Time
- Settlement
- Refund
- Reverse
- Approval
- Audit

Gate:

- integer Toman
- no floating money
- no historical mutation
- idempotent financial mutation
- audit exists
- rollback/reversal tested

---

# 14. Slice 5 — Transfer / Concurrency

این Slice یک مرحله امنیتی/بحرانی است.

Test scenarios:

1. Two operators transfer to same destination simultaneously.
2. Two Agents claim same station.
3. Old Agent connection sends heartbeat.
4. Session ends while transfer is occurring.
5. Transfer retries after network timeout.
6. DB transaction fails midway.
7. Operator repeats same command.

Expected:

- One authoritative winner
- No split ownership
- No duplicate settlement
- No phantom Agent
- No stale owner authority

---

# 15. Slice 6 — Tariff / VIP / Discount

- Tariff CRUD
- Pricing snapshots
- VIP plans
- VIP consumption
- Discount ceilings
- Approval

---

# 16. Slice 7 — Buffet / Inventory

- Products
- Purchases
- Sales
- Session sale
- Customer sale
- Warehouse
- Showcase
- Movement ledger
- Waste
- Return
- Adjustment
- Stock count
- Supplier

Critical test:

**Reverse Showcase sale must return stock to Showcase, not Warehouse.**

---

# 17. Slice 8 — Games / Accounts / Client Control

- Game catalog
- Game launch/stop
- Account pool
- Account lease
- Lock/unlock
- Kiosk
- Process detection
- Diagnostics
- Update

Agent remains execution-only.

---

# 18. Slice 9 — Users / Shift / Payroll / Cash

- Staff
- Roles
- Permissions
- Shift
- Cash Register
- Reconciliation
- Employee ledger
- Payroll
- Salary payment
- Approvals

---

# 19. Slice 10 — Reservation / Queue / Maintenance / Network

- Reservations
- Waitlist
- Maintenance
- Assets
- ISP1
- ISP2
- Network profiles
- Diagnostics

---

# 20. Slice 11 — Reports / Notifications / Audit Explorer

- Report Center
- Date filters
- Station filters
- Financial reports
- Customer reports
- Inventory reports
- Staff reports
- Audit Explorer
- Notification Center
- Authorized export

---

# 21. Slice 12 — Release / Backup / Recovery

- PostgreSQL backup
- Restore
- Verification
- Installer
- Desktop package
- Agent package
- Server package/service
- DataRoot
- Versioning
- Checksums
- Signing
- Update
- Rollback
- Real machine validation

---

# 22. Engineering Skill Map

این‌ها Skillهای اصلی ساخت محصول هستند؛ هر Slice باید فقط Skillهای مورد نیاز خودش را فعال کند.

## Skill 1 — Domain Modeling

توانایی لازم:

- Aggregate design
- Entity invariants
- Value Objects
- State transitions
- Domain errors
- Ownership rules

ممنوع:

- Setter آزاد روی همه‌چیز
- Business logic در UI
- Business logic در Controller

## Skill 2 — Application Use Cases

توانایی:

- Command
- Query
- Handler/Service
- Transaction boundary
- Authorization
- Idempotency

اصل:

**هر عملیات مهم یک Use Case مشخص دارد.**

## Skill 3 — PostgreSQL / EF Core

توانایی:

- Schema
- Migration
- Unique Index
- Foreign Key
- Transaction
- Concurrency
- PostgreSQL-specific behavior
- Query performance

## Skill 4 — API / Contract

توانایی:

- DTO
- Request/Response
- Validation
- Versioning
- Error contract
- Problem Details

## Skill 5 — Authentication / Authorization

توانایی:

- Password hashing
- Session/token lifecycle
- Permission
- Role
- Revocation
- Sensitive operation approval

## Skill 6 — SignalR / Agent Transport

توانایی:

- Authenticated connection
- ConnectionId
- DeviceId
- Heartbeat
- Lease
- Fencing
- Reconnect
- Ack
- Retry
- Disconnect

## Skill 7 — WPF Desktop

توانایی:

- MVVM/feature boundaries
- RTL
- Data binding
- Navigation
- Dialogs
- Commands
- Keyboard workflows
- Offline/Error states
- Live state

## Skill 8 — Windows Agent

توانایی:

- Windows Service
- Local secure storage
- Device identity
- Process control
- Service recovery
- Network reconnect
- Command execution

## Skill 9 — Financial Integrity

توانایی:

- Ledger
- Toman
- Settlement
- Reversal
- Debt
- Cash
- Idempotency
- Audit

## Skill 10 — Inventory Integrity

توانایی:

- Stock ledger
- Provenance
- Location
- Atomic decrement
- Return
- Reverse

## Skill 11 — Concurrency Engineering

توانایی:

- DB transactions
- Unique constraints
- optimistic concurrency
- pessimistic locking where required
- idempotency
- lease fencing

## Skill 12 — Testing

توانایی:

- Domain unit tests
- Application tests
- Integration tests
- Contract tests
- Persistence tests
- concurrency tests
- real PostgreSQL tests

## Skill 13 — Observability

توانایی:

- Structured logging
- correlation id
- operation id
- health
- diagnostics
- audit

## Skill 14 — Release Engineering

توانایی:

- Windows Service
- MSI/installer
- versioning
- manifest
- checksums
- signing
- rollback
- backup

---

# 23. Prompt Map — Promptهای استاندارد ساخت

این Promptها باید در هر Slice با context همان Slice استفاده شوند.

## 23.1 Master Architect Prompt

```text
You are building GameNet Manager 4.

Product:
Native Windows WPF CyberCafe management system.

Architecture:
WPF Desktop -> ASP.NET Core Server -> PostgreSQL
Windows Agent -> Server

Rules:
- Server is authoritative.
- Desktop is not business authority.
- Agent is execution/reporting only.
- PostgreSQL is production truth.
- No production mocks/fakes.
- No shadow browser runtime.
- Money uses integer Toman / explicit Money.
- Sensitive operations require permission and audit.
- DeviceId is durable identity; ConnectionId is transport state.
- Agent ownership uses lease/fencing.
- Financial and inventory history is append-only/reversible.
- Vertical slices must be fully tested before the next slice.

Do not redesign the product into Python, FastAPI, CLI, SQLite,
or browser-first architecture.

Before implementing:
1. Identify bounded responsibilities.
2. Identify Domain invariants.
3. Define Application use cases.
4. Define process boundary contracts.
5. Define persistence schema.
6. Define failure/concurrency behavior.
7. Define tests.
8. Define Desktop/Agent workflow.
9. Define completion gate.

Then implement the smallest complete vertical slice.
```

## 23.2 Domain Prompt

```text
For this GameNet capability:

[CAPABILITY]

Create only the Domain model first.

Output:
- entities
- value objects
- enums
- invariants
- state transitions
- domain exceptions/errors
- ownership rules
- concurrency assumptions

Do not put HTTP, EF Core, WPF, SignalR, or UI logic in Domain.

Every invalid state must be blocked by the domain where possible.
```

## 23.3 Application Prompt

```text
Implement the Application use cases for:

[CAPABILITY]

For every use case define:
- input
- authorization requirement
- validation
- domain operation
- transaction boundary
- persistence operation
- idempotency behavior
- concurrency behavior
- audit requirement
- output
- error contract

Do not let controllers or Desktop own business rules.
```

## 23.4 Persistence Prompt

```text
Implement PostgreSQL persistence for:

[CAPABILITY]

Requirements:
- explicit EF Core mapping
- appropriate indexes
- unique constraints
- foreign keys
- concurrency strategy
- transaction boundary
- migration
- no SQLite
- no in-memory production path
- no fake repository

Also add persistence tests against real PostgreSQL.
```

## 23.5 API Prompt

```text
Expose the Application use cases for:

[CAPABILITY]

Requirements:
- stable Contracts DTOs
- authentication
- authorization
- Problem Details
- validation
- correct HTTP semantics
- idempotency when mutation can be retried
- correlation id
- audit where required

Server endpoint must remain thin.
```

## 23.6 Desktop Prompt

```text
Build the WPF operator workflow for:

[CAPABILITY]

Rules:
- RTL-first Persian UI
- no business authority in Desktop
- real API data only
- loading state
- empty state
- offline state
- permission-denied state
- conflict state
- retry state
- keyboard support
- 1366x768 usable

The UI must expose server truth, not local fake state.
```

## 23.7 Agent Prompt

```text
Implement the Windows Agent behavior for:

[CAPABILITY]

Agent may:
- authenticate
- maintain transport
- heartbeat
- report telemetry
- execute authorized commands
- acknowledge results
- recover after reconnect

Agent may NOT:
- decide pricing
- decide customer ownership
- decide settlement
- bypass permissions
- write directly to PostgreSQL

Implement DeviceId, ConnectionId, lease/fencing and stale behavior
explicitly.
```

## 23.8 Test Prompt

```text
For [CAPABILITY], write tests before declaring completion.

Minimum:
- Domain invariant tests
- Application authorization tests
- persistence tests
- API contract tests
- integration tests
- retry/idempotency tests where relevant
- concurrency tests where relevant
- failure/recovery tests
- real PostgreSQL tests

Include the bug patterns learned from Repo 2 and Repo 3.
Do not use production mocks as a substitute for integration behavior.
```

## 23.9 Review Prompt

```text
Audit GameNet 4 feature [CAPABILITY] against the Master Build Catalog.

Check:
1. Domain integrity
2. Server authority
3. Desktop boundary
4. Agent boundary
5. PostgreSQL persistence
6. Authentication
7. Authorization
8. Audit
9. Concurrency
10. Idempotency
11. Recovery
12. UI states
13. Tests
14. Migration
15. Observability
16. No fake/shadow runtime
17. No giant file
18. No duplicated business rule

Return:
- PASS
- FAIL
- BLOCKED

For each failure cite file and exact missing behavior.
```

---

# 24. Slice Construction Template

هر Slice در Git باید این ساختار ذهنی را داشته باشد:

```text
Slice
│
├── Docs
│   ├── Rule
│   ├── Invariants
│   └── Completion Gate
│
├── Domain
│   ├── Entity
│   ├── ValueObject
│   └── Rules
│
├── Application
│   ├── Commands
│   ├── Queries
│   ├── Handlers/Services
│   └── Authorization
│
├── Contracts
│   ├── Request
│   ├── Response
│   └── Error
│
├── Infrastructure
│   ├── EF Mapping
│   ├── Migration
│   └── Adapters
│
├── Server
│   ├── Endpoint
│   └── Transport
│
├── Desktop
│   ├── View
│   ├── ViewModel
│   └── UX states
│
├── Agent
│   ├── Command
│   ├── Heartbeat
│   └── Recovery
│
└── Tests
    ├── Domain
    ├── Application
    ├── Persistence
    ├── Contract
    ├── Integration
    └── Concurrency
```

---

# 25. Bug Lessons Map from Repo 2/3

## جلوگیری از Giant Files

قانون:

- Program.cs = composition root
- endpointها feature-based
- هر Domain entity جدا
- Application services/use cases کوچک
- Desktop page split
- migrations کوچک و versioned

## Session Ownership

```text
Customer
   ↕
CustomerLogin
   ↕
Session
   ↕
Station
   ↕
AgentDevice
   ↕
Lease
```

این روابط باید همیشه سازگار باشند.

## Transfer Race

Transfer باید:

1. destination lock/claim
2. ownership validation
3. move Session
4. move CustomerLogin
5. move Agent lease relation
6. release source
7. commit

همه در یک transaction boundary معتبر.

## Identity Bug

- Customer identity ≠ Operator identity
- DeviceId ≠ ConnectionId
- StationId ≠ IP
- SessionId ≠ CustomerId

## SignalR Lifecycle

- reconnect-safe
- heartbeat
- lease version
- stale connection rejection
- server restart reconciliation

## Inventory Bug

Movement باید source/area/provenance داشته باشد.

Reverse نباید فقط Qty را +1 کند؛ باید همان stock lineage را اصلاح کند.

## Mock Runtime

هیچ Mock Service نباید جای source-of-truth production را بگیرد.

## Test Drift

Test باید رفتار واقعی Domain/Application/DB/API را تأیید کند، نه implementation قدیمی.

---

# 26. Definition of Done

یک Capability فقط زمانی Done است که:

### Product

- requirement مشخص است
- workflow مشخص است
- UI complete است

### Domain

- invariants وجود دارد
- invalid state قابل ایجاد نیست

### Application

- use case مستقل است
- authorization دارد
- transaction/idempotency مشخص است

### Persistence

- PostgreSQL واقعی
- migration
- FK/index/unique
- concurrency صحیح

### Server

- contract
- validation
- auth
- error handling
- audit

### Desktop

- real data
- loading
- empty
- offline
- conflict
- permission state
- RTL
- keyboard

### Agent

- execution
- heartbeat
- lease/fencing
- recovery
- no business authority

### Test

- unit
- integration
- persistence
- contract
- concurrency where relevant

### Operations

- logging
- diagnostics
- backup/recovery impact

---

# 27. Change Protocol

هر تغییر آینده:

1. Identify Slice.
2. Identify capability.
3. Update Domain rule if needed.
4. Add/update Application use case.
5. Add/update Contract.
6. Add persistence.
7. Add API/transport.
8. Add Desktop/Agent workflow.
9. Add tests.
10. Run CI.
11. Review diff.
12. Mark gate.
13. Only then continue.

### Commit rule

ترجیحاً:

```text
feat(domain): ...
feat(application): ...
feat(contract): ...
feat(infrastructure): ...
feat(server): ...
feat(desktop): ...
feat(agent): ...
test(...): ...
docs(...): ...
```

از commitهای بسیار بزرگ و نامرتبط اجتناب شود.

---

# 28. AI Working Protocol

وقتی از AI برای توسعه GameNet استفاده می‌شود:

### AI باید

- ابتدا architecture context را بخواند.
- Master Build Catalog را مرجع قرار دهد.
- از Repo 2/3 درس‌های ثبت‌شده را رعایت کند.
- یک Slice را کامل کند.
- diff کوچک نگه دارد.
- قبل از حرکت بعدی تست کند.
- خطا را دقیقاً ثبت کند.
- ادعای Build/Test را بدون نتیجه واقعی نکند.

### AI نباید

- Python/FastAPI را به جای .NET پیشنهاد دهد.
- SQLite را به جای PostgreSQL وارد production کند.
- fake/mock source ایجاد کند.
- business rule را داخل WPF بنویسد.
- Agent را business authority کند.
- Program.cs غول‌پیکر بسازد.
- یک Feature را فقط با UI «کامل» اعلام کند.
- چند Slice را همزمان و بدون gate اجرا کند.

---

# 29. Current Build Status — Repo 4

## Foundation

- .NET 10 pinned
- WPF
- ASP.NET Core
- PostgreSQL/Npgsql
- EF Core
- Contracts
- Domain Money
- Clock
- Server health
- Agent Worker
- CI
- Domain test

## Slice 1 started

Implemented foundation pieces:

- Operator roles/permissions
- Operator entity
- Operator session
- Station entity
- Station types/lifecycle
- AgentDevice
- Durable DeviceId
- Pairing
- Agent credential
- LeaseVersion
- ConnectionId
- Heartbeat
- Stale behavior
- Station binding
- Application services
- Contracts
- PostgreSQL mapping
- secure password/token primitives

### Still required before Slice 1 Gate

- complete DB migration path
- Server auth endpoints
- Server Agent transport
- Operator authorization middleware/use
- Desktop login UI
- Desktop station UI
- real Agent pairing/heartbeat client
- integration tests
- concurrency/fencing tests
- full CI verification
- migration/startup verification

**بنابراین Slice 1 هنوز Complete نیست.**

---

# 30. Final Product Map

```text
                     GAMENET MANAGER 4
                              │
        ┌─────────────────────┼──────────────────────┐
        │                     │                      │
     Desktop                Server                Agent
        │                     │                      │
        │               Business Authority           │
        │                     │                      │
        │          ┌──────────┼──────────┐           │
        │          │          │          │           │
        │       Domain   Application  Contracts      │
        │          │          │          │           │
        │          └──────────┼──────────┘           │
        │                     │                      │
        └─────────────────────┼──────────────────────┘
                              │
                         Infrastructure
                              │
                         PostgreSQL
                              │
       ┌──────────────────────┼─────────────────────────┐
       │          │            │             │            │
   Customers   Sessions     Billing      Inventory    Audit
       │          │            │             │            │
      VIP       Tariff       Wallet       Buffet       Reports
       │          │            │             │            │
       └──────────┴────────────┴─────────────┴────────────┘

Execution order:
0 Foundation
1 Operator + Station + Agent
2 Customer
3 Session
4 Billing/Wallet
5 Transfer/Concurrency
6 Tariff/VIP
7 Buffet/Inventory
8 Games/Client Control
9 Staff/Shift/Payroll
10 Reservation/Maintenance/Network
11 Reports/Notifications/Audit
12 Backup/Release/Recovery
```

---

# 31. One-line Product Contract

> **GameNet Manager 4 باید یک سیستم Desktop واقعی، Server-authoritative، PostgreSQL-backed، modular و قابل نصب برای مدیریت کامل CyberCafe باشد؛ نه مجموعه‌ای از صفحات UI و نه یک prototype.**

---

# 32. Master Prompt برای شروع هر جلسه ساخت

```text
You are continuing the construction of GameNet Manager 4.

Read and obey:
docs/product/gamenet-master-build-catalog.md

Current architecture:
WPF Desktop
→ ASP.NET Core Server
→ PostgreSQL
Windows Agent
→ Server

Do not change the architecture.

Work only on:
[S L I C E]
[CAPABILITY]

Before coding:
1. identify the domain rule
2. identify invariants
3. identify use cases
4. identify contracts
5. identify persistence
6. identify permissions
7. identify audit
8. identify concurrency/idempotency
9. identify Desktop workflow
10. identify Agent workflow
11. define tests
12. define completion gate

Then implement one complete vertical slice.

Never:
- create Python/FastAPI/CLI
- introduce SQLite
- create production mocks
- create a shadow runtime
- put business logic in Desktop
- make Agent the authority
- bypass Server
- mutate historical financial truth
- create giant files

Do not call a capability complete until the Definition of Done
and its Slice Gate are satisfied.

Be explicit about what is completed, what is blocked, and what remains.
Never claim a build/test result without an actual verified result.
```
