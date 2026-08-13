# Upgrade Options — MoopelFrontend

Assessment: 4 projects already on modern .NET 10; only framework-bump and behavioral change signals surfaced.

## Strategy

### Upgrade Strategy
The solution is small, fully on modern .NET, and the assessment only surfaced framework upgrade work plus .NET 11 behavioral changes. An all-at-once upgrade keeps the change set coherent across the client, server, shared, and test projects.

| Value | Description |
|-------|-------------|
| **All-At-Once** (selected) | Upgrade all projects simultaneously in a single atomic operation. |
