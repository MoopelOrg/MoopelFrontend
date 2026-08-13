# .NET Version Upgrade Scenario

## Strategy
All-At-Once — upgrade all projects simultaneously in a single atomic operation.

## Preferences
- **Flow Mode**: Automatic
- **Commit Strategy**: After Each Task
- **Pace**: Standard
- **Target Framework**: .NET 11 (preview)

## Source Control
- **Source Branch**: main
- **Working Branch**: upgrade-dotnet-11
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: All-At-Once
