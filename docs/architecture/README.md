# GameNet 4 Architecture

## Runtime

Desktop -> Server/API -> PostgreSQL

Agent -> authenticated Server transport -> Server

The Desktop is an operator client. The Agent is an execution/reporting client. Neither is an accounting authority.

## Boundaries

- Domain contains invariants and value objects.
- Application contains use-case contracts and orchestration boundaries.
- Infrastructure contains persistence/integration adapters.
- Server contains the HTTP composition root and transport endpoints.
- Desktop contains operator UX only.
- Agent contains machine execution/reporting only.
- Contracts contains DTOs crossing process boundaries.

## Construction rule

Each product slice must introduce a real persistence path and tests before it is considered complete. Fake data, production mocks and a second shadow runtime are not permitted.
