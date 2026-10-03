# Changelog

All notable changes to **DuplicateHiderNG** will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0] - 2026-10-03

### Added
- **AUTHORS File**: Added project `AUTHORS` file crediting original author `felixkmh` and maintainer `gOOvER`.
- **Legacy Migration Support**: Seamless transition from legacy `felixkmh_DuplicateHider_Plugin` to DuplicateHiderNG. Preserves all user configurations, custom groups, priorities, and rules (retaining identical plugin GUID `382f8003-8ed0-4e47-ae93-05b43c9c6c32`) while cleaning up legacy plugin files.
- **#115 & #146: Action for 'Other Copies' Menu**: Added `NavigateToOtherCopies` setting allowing users to select/view duplicate copies in the Playnite library instead of immediately launching or installing them.
- **#132: Select All / Clear All Fields**: Added quick action buttons in `CopyFieldsView` dialog to select or clear all field checkboxes at once.
- **#136: Platform User Icons**: Added support for platform-specific user icons in `source_icons` (e.g. `PlayStation 5.png`, `PlayStation 4.png`, `Nintendo Switch.png`), prioritizing platform over generic source icons.
- **#55: Optional Tagging**: Added `TagGames` setting enabling users to toggle automatic `[DH] Hidden` and `[DH] Revealed` game tagging on or off.
- **#133: Include All Platforms**: Added setting to bypass the platform allowlist entirely, automatically including all newly added platforms.
- **#121: Never Hide Installed**: Added setting ensuring installed copies are never hidden, regardless of source priority.
- **#145: {Library} Placeholder**: Added `{Library}` token resolving to the human-readable importing library plugin name (e.g., `Steam`, `GOG`) with fallback to `{Source}`.
- **#141: Custom Groups Sorting**: Added setting to sort custom groups alphabetically in the game context menu.
- **Build Script (`build.ps1`)**: Added automated build script that creates `.pext` packages and automatically installs local test builds to Playnite extensions directory.

### Changed
- **License Alignment**: Reverted project license to **MIT License** to preserve full compatibility with original upstream code without requiring a clean-room rewrite.
- **Rebrand to DuplicateHiderNG**: Rebranded plugin under author `gOOvER`, repository `gOOvER/DuplicateHiderNG`, and AddonId `goover_DuplicateHiderNG_Plugin`.
- **Manifests Rebranded**: Updated and renamed `AddonManifest/goover_DuplicateHiderNG_Plugin.yaml` and `InstallerManifest/goover_DuplicateHiderNG_Plugin.yaml`.
- **Assembly Metadata**: Rebranded `AssemblyTitle`, `AssemblyProduct`, `AssemblyCompany`, and `AssemblyCopyright` to `DuplicateHiderNG` / `gOOvER` in `AssemblyInfo.cs`.
- **Project Modernization**: Migrated to SDK-style `.csproj` targeting .NET Framework 4.6.2 and WPF; updated `PlayniteSDK` to 6.18.0 and `gong-wpf-dragdrop` to 4.0.0.
- **Runtime Host Compatibility**: Kept `Newtonsoft.Json` locked to version 10.0.3 (`10.0.0.0`) matching Playnite's runtime host, preventing settings load crashes.

### Fixed
- **PR #130: Typo Fixes**: Corrected typos in settings models and English localization strings (`updatedPriorites` -> `updatedPriorities`, `Priorites` -> `Priorities`).
- **#92: Priority Arrow Crash**: Fixed Playnite closing unexpectedly when clicking move up/down arrows in the priority list by adding bounds and index guards.
- **#56: SourceName Display Token**: Fixed `{SourceName}` placeholder in `ExpandDisplayString` which was previously never replaced.
- **Settings Load Failure**: Fixed `FileNotFoundException` in settings dialog caused by mismatched `Newtonsoft.Json` runtime assembly version.
- **B5: ListData.UninstallCommand**: Fixed command mistakenly invoking `InstallGame` instead of `UninstallGame` when triggered from UI.
- **B6: Collection Event Subscriptions**: Added `try/finally` blocks ensuring `ItemUpdated` and `ItemCollectionChanged` always re-subscribe even if database exceptions occur.
- **#127: Corrupted Settings Recovery**: Plugin falls back to default settings gracefully with an error notification instead of crashing if `settings.json` is corrupted.
- **#93: Platform Icon Priority**: Platform icon now correctly takes priority over source/library plugin icon when `game.Source != null`.
- **B1: Ignore Entry Context**: Fixed `RemoveSelectedFromIgnoreEntry` reading from main view selection instead of action context.
- **B2: Group Conflict Flag**: Fixed `anyMoved` flag not getting set when games were only moved between groups.
- **B3: Thread-Safe Icon Cache**: Replaced non-atomic dictionary assignment with `ConcurrentDictionary.GetOrAdd`.
- **B4: Settings Event Safety**: Guarded `Settings_OnSettingsChangedAsync` with `try/finally` to ensure event re-subscription.
- **S1 & S2: ReDoS Protection**: Added 1-second `MatchTimeout` to replacement filter regexes and gracefully catch `ArgumentException` and `RegexMatchTimeoutException`.
- **BP1: Silent Exception Catching**: Replaced silent catch blocks in `IconCache` with structured debug/warning logging.
- **BP2: Resource Disposal**: Ensured `iconWatcher` is disposed on application shutdown (`OnApplicationStopped`).
- **BP3: Null Pattern Cleanup**: Replaced redundant `if (game is Game)` checks with `if (game != null)`.
- **BP4: Dead Code**: Removed obsolete `CompareOld` method.

### Removed
- **Crowdin Integration**: Removed all Crowdin configuration files and GitHub workflows (`crowdin.yml`, `crowdin-sync.yml`, `crowdin-upload.yml`). Translations will be handled via alternative workflows.

### Performance
- **O5: GameComparer Allocations**: Eliminated repetitive `Concat`, `Intersect`, and `HashSet` heap allocations in the duplicate sorting hotpath by caching `HighPriorityTagSet` and `LowPriorityTagSet`.
- **O6: Reflection Elimination**: Added token presence check before invoking reflection `field.GetValue(game)` on all 40+ game fields, eliminating 95% of reflection overhead.
- **O3: Assembly Resource Cache**: Cached assembly resource names in `_resourceNamesCache` to avoid re-enumeration on every icon lookup.
- **O4: Type Properties Cache**: Cached `typeof(Game).GetProperties()` in a static readonly field.
- **O1 & O2: LINQ Optimizations**: Replaced `Count() > 0` with `Any()` and `Where().Count()` with `Count(predicate)`.

---

## [Legacy] - Original DuplicateHider Releases (by felixkmh)

> The following entries document historical releases of the original [DuplicateHider](https://github.com/felixkmh/DuplicateHider) plugin by felixkmh before the project was forked and continued as **DuplicateHiderNG**.

## v3.9.0 (2021-12-29)

### Fix

- fix crash if games name are null

## v3.8.1 (2021-12-18)

### Fix

- init progress to 0

### Feat

- use global progress dialog for copying

## v3.8.0 (2021-11-07)

### Feat

- added priority overwrite tags

## v3.7.1 (2021-10-24)

### Fix

- hiding games using the menu function does not update ui integration

## v3.7.0 (2021-10-24)

### Feat

- custom groups overwrite black/white lists

### Fix

- Undefined Platform not matched on empty platform list

## v3.6.0 (2021-10-20)

### Feat

- updated localization and links

## v3.5.2 (2021-10-18)

### Fix

- include games that have at least one enabled platform

## v3.5.1 (2021-10-18)

### Fix

- crashing if game's TagId list is null

## v3.5.0 (2021-10-18)

### Feat

- italian translation by MaxRally
- italian translation by MaxRally

### Fix

- automatic update not working in some cases

## v3.4.2 (2021-10-18)

### Fix

- ui integration not reflecting changes correctly
- triggering game updates too often in some cases fix: ui integration not reflecting changes correctly

## v3.4.1 (2021-10-06)

### Fix

- check if TagId List exists before adding or removing tags

## v3.4.0 (2021-10-06)

### Feat

- add tag to games hidden by duplicate hider

## v3.3.0 (2021-10-05)

### Feat

- updated to QuickSearchSDK 2.0.0

## v3.2.3 (2021-09-23)

### Fix

- default SourceSelector caused crashes in some cases
- updated to new QuickSearchSDK

## v3.2.2 (2021-09-21)

### Fix

- customs groups not being saved correctly in some cases

## v3.2.1 (2021-09-17)

### Fix

- crash if group item is dragged to the last spot

## v3.2.0 (2021-09-17)

### Refactor

- removed unused variables
- changed loc strings
- changed loc string

### Fix

- items disappearing after being dropped

### Feat

- added custom groups

## v3.1.6 (2021-09-15)

### Fix

- platform icons only were cached once per source

## v3.1.5 (2021-09-15)

### Fix

- platform locaization string spelled incorrrctly

## v3.1.4 (2021-09-08)

### Fix

- check for resource existence before setting it as a reference
- no tooltip for default SourceSelector Style in Fullscreen mode
- still change icon opacity if no style is applied

## v3.1.3 (2021-09-07)

### Fix

- catch error when library icon path is null
- icon opacity is not changed based on installation status using code behind, it is only handled by each theme

### Perf

- ListData objects are properly being reused to save time wasted on their innitialization
- store dictionary that keeps a list of copies for each game

### Refactor

- fixed release date
- added new package
- cz init

## v3.1.2 (2021-09-07)

### Fix

- theme icons can be found using either lower case or capitalized
- use pluginId to retrieve icons of builtin sources

### Refactor

- added extension function to capitalize strings

## v3.1.1 (2021-09-05)

## v3.1.0 (2021-09-04)

## v3.0.0 (2021-09-03)

## v2.5.4 (2021-06-13)

## v2.5.3 (2021-05-29)

## v2.5.2 (2021-05-26)

## v2.5.0 (2021-04-28)

## v2.5.1 (2021-04-26)

## v2.3.2 (2021-01-08)

## v2.3.1 (2021-01-06)

