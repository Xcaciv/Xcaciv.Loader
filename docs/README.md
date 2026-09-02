# Documentation

Current reference documentation for Xcaciv.Loader. Start at the [root README](../README.md) for a quick-start; the docs below go deeper on specific topics.

| Document | What it covers |
|----------|-----------------|
| [security-features-v2.md](security-features-v2.md) | Security policies, dynamic-assembly blocking, preflight checks, global monitoring, path validation, and integrity verification — how to configure the library securely. |
| [multi-framework.md](multi-framework.md) | Building and testing against .NET 8.0 (default) or .NET 10.0. |
| [MIGRATION-v1-to-v2.md](MIGRATION-v1-to-v2.md) | Moving from the deprecated static `AssemblyContext.SetStrictDirectoryRestriction` API to the v2.0 instance-based `AssemblySecurityPolicy` API. |
| [TESTING-QUICK-START.md](TESTING-QUICK-START.md) | Running and extending the test suite, including the dynamic-assembly-monitoring test infrastructure. |
| [../CHANGELOG.md](../CHANGELOG.md) | Version history. |

## Archive

[archive/](archive/) holds superseded and historical documents — point-in-time
design specs, implementation write-ups, and release reports that no longer
describe the current codebase but are kept for context. See
[archive/README.md](archive/README.md) for an index and why each one is there.
