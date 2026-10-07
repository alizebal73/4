# Module dependency rules

Allowed direction:

`Api -> Application -> Domain`

`Infrastructure -> Application/Domain`

Feature-to-feature calls use an application's public contract. They do not use another feature's entities, repositories or infrastructure types.

Server cross-cutting infrastructure is technical support, not a business owner.

Desktop depends on Shared contracts and the Server transport boundary. It does not reference Server implementation assemblies.

Agent depends on Shared contracts and its local execution adapters. It does not reference Server implementation assemblies.

Persistence is an implementation boundary used by Server infrastructure/modules; generated migrations remain centralized while entity ownership stays with the feature that owns the invariant.
