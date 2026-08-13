# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade the MoopelFrontend solution from .NET 10 to .NET 11 preview across the client app, server app, shared library, and test project.
**Scope**: 4 SDK-style projects already on modern .NET, so this is a straightforward all-at-once TFM bump with minor API and behavioral fixes expected.

## Tasks

### 01-upgrade-solution: Upgrade all projects to .NET 11

All projects are already on modern .NET, so this upgrade can be handled as a single atomic change set. Update the target frameworks for the Blazor client, server app, shared library, and tests together so the solution stays coherent while framework-specific behavior changes are resolved in the same pass. Begin by verifying the .NET 11 toolchain and repository-level version constraints, then update the project files, align package references if any become incompatible, and fix any code or test failures introduced by the new framework behavior.

**Done when**: every project targets .NET 11, the solution restores and builds successfully, the test project passes, and no upgrade-related compilation errors remain.
