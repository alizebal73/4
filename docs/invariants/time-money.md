# Time and Money Invariants

1. Canonical money unit is Toman.
2. Money is represented as an integer or explicit money value object; floating point is not authoritative.
3. Server time is the authoritative time source for billable operations.
4. Desktop/Agent clocks are telemetry only until the Server accepts a timestamp.
5. Historical financial facts are never corrected by mutating old records; future operations create compensating entries.
