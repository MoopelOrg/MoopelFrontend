# 01-upgrade-solution: Upgrade all projects to .NET 11

## Scope Inventory
- **Projects affected**: `MoopelFrontend.Client`, `MoopelFrontend`, `MoopelFrontend.Shared`, `MoopelFrontend.Tests`
- **Distinct concerns**: framework retargeting, framework-behavior fixes, package alignment, test validation
- **Change signals**:
  - All four projects currently target `net10.0`
  - Assessment flagged `Project.0002` in every project for retargeting to `net11.0`
  - Assessment flagged `Api.0003` in all projects; the only concrete code-level hits were `Uri`/`HttpContent` usage in startup, shared API service, tests, and generated Blazor code
- **Skill matches**: managing-target-frameworks, modifying-project-properties, building-projects

All projects are already on modern .NET, so this upgrade can be handled as a single atomic change set. Update the target frameworks for the Blazor client, server app, shared library, and tests together so the solution stays coherent while framework-specific behavior changes are resolved in the same pass. Begin by verifying the .NET 11 toolchain and repository-level version constraints, then update the project files, align package references if any become incompatible, and fix any code or test failures introduced by the new framework behavior.

## Research Notes
- Dependency graph shows the client references `MoopelFrontend.Shared`, the server references both `MoopelFrontend.Shared` and `MoopelFrontend.Client`, and the test project references all three production projects.
- Package inventory is already aligned to the .NET 10 wave; the assessment did not report incompatible packages or security vulnerabilities.
- The task was initially blocked by the missing .NET 11 SDK in this environment, but the SDK is now available and the project files have been retargeted to `net11.0`.
- Package references were left unchanged because the assessment reported them as compatible with the new framework.

**Done when**: every project targets .NET 11, the solution restores and builds successfully, the test project passes, and no upgrade-related compilation errors remain.
