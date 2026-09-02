# Archived Documentation

This directory holds documents that describe a past state of the project
rather than its current state — original design specs whose details have
since diverged from the code, and point-in-time implementation or release
reports. They're kept for historical context and audit trail, not as current
reference material. For current documentation, start at
[../README.md](../README.md).

## Design history

- **[spec-architecture-dynamic-assembly-loading.md](spec-architecture-dynamic-assembly-loading.md)** — the original v1.0 architecture/requirements specification (2025-10-12). Still the source of the library's core goals (isolation, unloading, path verification), but its printed `IAssemblyContext` interface contract predates `IAsyncDisposable`, `UnloadAsync()`, and `EnableGlobalDynamicAssemblyMonitoring()`. For the current interface, read the source or [security-features-v2.md](../security-features-v2.md).
- **[ssem-scoring-methodology.md](ssem-scoring-methodology.md)** — explains the SSEM (Securable Software Engineering Model) framework and scores the v1.x → v2.0 transition. The methodology section has lasting reference value; the scores themselves predate v2.1 and aren't maintained.
- **[spec-ssem-improvement-checklist-20251129.md](spec-ssem-improvement-checklist-20251129.md)** — the item-by-item SSEM improvement tracking checklist (REL-001, TRUST-001, etc.) behind the v2.0 effort. Preserves design rationale for items that were deferred, including some — like an `IPathValidator` extraction — that are still unimplemented and worth knowing about if picking up future work.
- **[plan-v2.1-enhancements.md](plan-v2.1-enhancements.md)** — the v2.1+ backlog proposed at the time. Some of what it lists as future work has already shipped (e.g. `LoadTimeout` support); check [../../CHANGELOG.md](../../CHANGELOG.md) before treating anything here as still open.
- **[docs-index-2025-12.md](docs-index-2025-12.md)** — the documentation index as it stood in December 2025, kept as a record of that structure. Superseded by [../README.md](../README.md).

## Release history

- **[V2.0-Release Notes.md](V2.0-Release%20Notes.md)** — the consolidated v2.0.0 release report (security architecture, metrics, breaking changes). Genuinely the record of that milestone; the package has since moved to 2.1.x — see [../../CHANGELOG.md](../../CHANGELOG.md) for what shipped after.
- **[NUGET-RELEASE-SUMMARY.md](NUGET-RELEASE-SUMMARY.md)** — pre-publish checklist for the v2.0.0 NuGet package. All its pending items were resolved before release.

## Implementation write-ups

- **[trust-001-implementation-summary.md](trust-001-implementation-summary.md)** — design rationale for the assembly-integrity-verification feature (`AssemblyHashStore`, `AssemblyIntegrityVerifier`), including why CSV was chosen over JSON (to avoid `IL2026` trimming warnings) and an attack-scenarios-mitigated table not found elsewhere. Current usage guidance for the same feature lives in [security-features-v2.md](../security-features-v2.md).
- **[phase1-final-summary.md](phase1-final-summary.md)** — the completed record of "Phase 1" of the SSEM improvement effort (5 items), with before/after metrics.
- **[phase3-testing-complete-summary.md](phase3-testing-complete-summary.md)** — includes the before/after code for a `DisposeAsync` concurrency fix; kept as the only recorded rationale for that fix.

## Why these and not others

Several files that once lived here (and some that lived in `docs/` and the
repo root) were deleted rather than archived: duplicate or superseded
drafts of the reports above, and one-time AI-generated build/test status
snapshots with no information not already captured, more durably, in git
history and [../../CHANGELOG.md](../../CHANGELOG.md). Archiving is for
material with lasting design or audit value that current docs don't fully
carry forward; near-duplicates and pure process exhaust aren't kept just
because they existed once.
