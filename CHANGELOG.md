# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

Nothing yet.

## [2.1.2] - 2025-12-29

### Changed
- Refactored `build.ps1` to unify the build and test steps for .NET 8.0 and .NET 10.0, reducing duplication and improving output clarity
- `Xcaciv.Loader.csproj` now uses conditional `TargetFrameworks` for multi-targeting; pass `/p:UseNet10=true` to `dotnet build`/`build.ps1` to also build for .NET 10.0 (see [docs/multi-framework.md](docs/multi-framework.md))
- Refactored `ThreadSafetyTests` for more reliable async handling
- Realigned package license metadata

## [2.1.1] - 2025-12-26

### Added
- Comprehensive test infrastructure for the security features shipped in 2.1.0:
  - Test assemblies `zTestRiskyAssembly` (demonstrates `Reflection.Emit`) and `zTestLinqExpressions` (demonstrates `Expression.Compile`)
  - `GlobalDynamicAssemblyMonitoringTests` covering basic monitoring, multi-context isolation, weak-reference cleanup, thread-safety, and event content
  - `DisallowDynamicAssembliesTests` covering policy configuration, multi-context isolation, and policy inheritance under `Strict`

### Changed
- Added `InternalsVisibleTo` for the test project so it can exercise the internal `AssemblyPreflightAnalyzer`

## [2.1.0] - 2025-12-22

### Added
- New security policy flag: `AssemblySecurityPolicy.DisallowDynamicAssemblies` to prohibit loading dynamic/in-memory assemblies (e.g., created via Reflection.Emit such as `AssemblyBuilder`, `TypeBuilder`, `DynamicMethod`).
- `AssemblyContext.EnableGlobalDynamicAssemblyMonitoring()`: opt-in, process-wide monitoring that raises `SecurityViolation` when a dynamic (in-memory) assembly is created anywhere in the `AppDomain` while a `Strict`-policy context is subscribed. Uses weak references internally so subscribing contexts don't leak.
- `AssemblyPreflightAnalyzer` (internal): lightweight, non-executing metadata scan that flags `Reflection.Emit` namespace usage and `LINQ.Expressions.Compile` patterns in an assembly file under `StrictMode`, before the assembly is loaded.

### Changed
- `AssemblyContext` enforces `DisallowDynamicAssemblies` during load operations (`LoadFromPath`, `LoadFromName`): raises `SecurityViolation` and throws `SecurityException` when policy disallows dynamic assemblies.
- `AssemblySecurityPolicy.Strict` enables `DisallowDynamicAssemblies` by default; `Default` leaves it disabled for compatibility.
- Refactored `AssemblyHashStore.MergeFromFile` to use an explicit LINQ `Where` filter for clarity.
- Fixed an always-false condition in `UnloadAsync` that could prevent proper cleanup.

### Security
- Blocks dynamic/in-memory assembly loads under strict policy to reduce runtime injection risk.

### Notes
- No breaking changes for `Default` policy users. `Strict` may now throw `SecurityException` when attempting to load dynamic assemblies.

## [2.0.0] - 2025-11-30

### Added
- **Instance-based Security Policies**: New `AssemblySecurityPolicy` class provides configurable security policies per `AssemblyContext` instance
  - `AssemblySecurityPolicy.Default`: Basic system directory restrictions
  - `AssemblySecurityPolicy.Strict`: Enhanced restrictions for high-security environments
  - Support for custom forbidden directory lists
  - `SecurityPolicy` property on `AssemblyContext` for per-instance configuration
- **Assembly Integrity Verification**: Optional cryptographic hash-based verification (disabled by default)
  - `AssemblyIntegrityVerifier` class with learning and strict modes
  - `AssemblyHashStore` for managing hashes with CSV persistence
  - Support for SHA256, SHA384, and SHA512 hash algorithms
  - Learning mode automatically trusts new assemblies on first load
  - Strict mode only loads assemblies with known hashes
  - Events for hash mismatches and hash learning
  - Simple CSV file format (no external dependencies)
- **Path Validation Utilities**: New `AssemblyPathValidator` class for input sanitization
  - `SanitizeAssemblyPath()`: Remove dangerous characters, normalize separators
  - `ResolveRelativeToBase()`: Resolve relative paths to application base
  - `IsSafePath()`: Basic heuristic safety checks
  - `HasValidAssemblyExtension()`: Validate .dll or .exe extension
  - `ValidateAndSanitize()`: Combined validation pipeline (recommended)
- **Type Discovery Utilities**: New `AssemblyScanner` class for clean type scanning
  - `GetLoadedTypes<T>()`: Scan all loaded assemblies in AppDomain
  - `GetTypes<T>(Assembly)`: Scan specific assembly
  - Better organization than mixing with `AssemblyContext`
- **Enhanced Exception Handling**: Specific exception types now caught and wrapped with context (`MissingMethodException`, `TargetInvocationException`, `MemberAccessException`, `TypeLoadException`)
- **Audit Trail Events**: All security violations and dependency resolutions now raise events before throwing exceptions

### Changed
- **BREAKING**: Security configuration is now instance-based instead of static. Each `AssemblyContext` can have its own `SecurityPolicy`, enabling parallel test execution without interference and removing shared mutable state.
- **BREAKING**: Silent failures eliminated - security exceptions now always propagate after raising events. Dependency resolution failures are now visible to callers.
- Null checking standardized to use `ArgumentNullException.ThrowIfNull()` / `ArgumentException.ThrowIfNullOrWhiteSpace()` throughout
- Exception handling improved: replaced broad catch-and-swallow patterns with specific exception types and proper context wrapping

### Deprecated
- `AssemblyContext.SetStrictDirectoryRestriction(bool)`: Use `AssemblySecurityPolicy` parameter in constructor instead. **Removal planned**: v3.0.0.
- `AssemblyContext.IsStrictDirectoryRestrictionEnabled()`: Use `AssemblyContext.SecurityPolicy.StrictMode` instead. **Removal planned**: v3.0.0.
- `AssemblyContext.GetLoadedTypes<T>()`: Use `AssemblyScanner.GetLoadedTypes<T>()` instead. **Removal planned**: v3.0.0.

See [docs/MIGRATION-v1-to-v2.md](docs/MIGRATION-v1-to-v2.md) for the full migration guide with before/after code samples.

### Security
- Enhanced path validation with instance-based forbidden directory lists
- Comprehensive audit events for all security-related operations
- Optional cryptographic integrity verification
- Input sanitization utilities
