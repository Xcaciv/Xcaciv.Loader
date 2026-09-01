# Dynamic Assembly Monitoring - Testing Infrastructure

## Quick Start

### Run All Tests
```powershell
cd src/Xcaciv.LoaderTests
dotnet test
```

### Run Specific Test Suite
```powershell
# Test DisallowDynamicAssemblies functionality
dotnet test --filter "ClassName=DisallowDynamicAssembliesTests"

# Test GlobalDynamicAssemblyMonitoring functionality  
dotnet test --filter "ClassName=GlobalDynamicAssemblyMonitoringTests"
```

### Run Specific Test
```powershell
dotnet test --filter "Name~EnableGlobalDynamicAssemblyMonitoring_WithStrictPolicy_RaisesSecurityViolation"
```

## Project Organization

### Test Assemblies

#### zTestRiskyAssembly
Demonstrates Reflection.Emit usage and dynamic type creation:
```csharp
// Key behavior: Creates dynamic types using AssemblyBuilder
var risky = new zTestRiskyAssembly.DynamicTypeCreator();
string result = risky.Stuff("input"); // Creates AssemblyBuilder dynamically
```

**Detected By**:
- AssemblyPreflightAnalyzer (static analysis)
- GlobalDynamicAssemblyMonitoring (runtime monitoring)

#### zTestLinqExpressions
Demonstrates LINQ.Expressions.Compile usage:
```csharp
// Key behavior: Compiles expression trees at runtime
var compiler = new zTestLinqExpressions.ExpressionCompiler();
string result = compiler.Stuff("input"); // Compiles expressions dynamically
```

**Detected By**:
- AssemblyPreflightAnalyzer (static analysis)
- GlobalDynamicAssemblyMonitoring (runtime monitoring)

### Test Suites

#### DisallowDynamicAssembliesTests (12 tests)
Tests the `DisallowDynamicAssemblies` security policy property:
- Policy configuration validation
- Context configuration with different policies
- Multi-context isolation
- Property immutability
- Policy inheritance

#### GlobalDynamicAssemblyMonitoringTests (10 tests)
Tests the `EnableGlobalDynamicAssemblyMonitoring()` method:
- Basic monitoring functionality
- Strict vs Default policy behavior
- Multi-context event delivery
- Thread-safety
- Weak reference cleanup
- Integration with risky assemblies

## Test Execution Workflow

### 1. Security Policy Tests
```csharp
// Verify Strict policy blocks dynamics
var strictPolicy = AssemblySecurityPolicy.Strict;
Assert.True(strictPolicy.DisallowDynamicAssemblies);

// Verify Default policy allows dynamics
var defaultPolicy = AssemblySecurityPolicy.Default;
Assert.False(defaultPolicy.DisallowDynamicAssemblies);
```

### 2. Runtime Monitoring Tests
```csharp
// Create context with Strict policy
using var context = new AssemblyContext(
    path, 
    basePathRestriction: "*",
    securityPolicy: AssemblySecurityPolicy.Strict);

// Enable global dynamic assembly monitoring
context.EnableGlobalDynamicAssemblyMonitoring();

// Subscribe to security violations
bool violated = false;
context.SecurityViolation += (id, msg) => { violated = true; };

// Create dynamic assembly
var assemblyName = new AssemblyName("DynamicAssembly");
AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

// Verify violation was raised
Assert.True(violated);
```

### 3. Integration Tests  
```csharp
// Test with actual risky assembly
var risky = new zTestRiskyAssembly.DynamicTypeCreator();
var result = risky.Stuff("test"); // Creates dynamic type

// Context with monitoring will detect it
// Verifies end-to-end workflow
```

## Key Test Scenarios

### Scenario 1: Static Detection with Preflight Analysis
```
Input: zTestRiskyAssembly.dll
Process: AssemblyPreflightAnalyzer.Analyze()
Output: HasAnyIndicators = true
Details: Detects Reflection.Emit usage
Status: Works without loading the assembly
```

### Scenario 2: Runtime Monitoring with Strict Policy
```
Setup: AssemblyContext with Strict policy
Enable: GlobalDynamicAssemblyMonitoring()
Action: Execute zTestRiskyAssembly code
Result: SecurityViolation event raised
Status: Detects dynamic assembly creation
```

### Scenario 3: Silent Operation with Default Policy
```
Setup: AssemblyContext with Default policy
Enable: GlobalDynamicAssemblyMonitoring()
Action: Create dynamic assembly with AssemblyBuilder
Result: No SecurityViolation event
Status: Correct policy-based behavior
```

### Scenario 4: Thread-Safe Multi-Context Monitoring
```
Setup: 10 contexts with Strict policy
Action: All call EnableGlobalDynamicAssemblyMonitoring() concurrently
Create: 1 dynamic assembly
Result: All 10 contexts receive SecurityViolation event
Status: Thread-safe, no duplicates
```

### Scenario 5: Memory-Safe Cleanup
```
Setup: Create context, enable monitoring, dispose
GC: Force garbage collection
Create: New dynamic assembly
Result: Disposed context doesn't receive event
Status: Weak references prevent memory leaks
```

## Design Notes

### Weak References in GlobalDynamicAssemblyMonitoring
The global monitor uses `WeakReference<AssemblyContext>` to avoid memory leaks:
- Contexts can be disposed without affecting the global subscription list
- Disposed contexts are automatically cleaned up when GC runs
- No explicit deregistration required

### Thread Safety
The global monitor uses a lock (`globalMonitorLock`) to protect:
- Subscriber list modifications
- Handler subscription/unsubscription
- List cleanup

This ensures thread-safe registration and event delivery.

### Policy Inheritance
- Strict policy enables both `DisallowDynamicAssemblies` and more forbidden directories
- Custom policies can be created with specific forbidden directories
- Each context instance can have its own policy
- No global state (instance-based configuration)

## Security Implications

The test assemblies (`zTestRiskyAssembly` and `zTestLinqExpressions`) intentionally demonstrate risky patterns:

1. **zTestRiskyAssembly**: Uses `AssemblyBuilder` to create types at runtime — demonstrates Reflection.Emit usage, and allows testing of both static preflight detection and runtime dynamic-assembly monitoring.
2. **zTestLinqExpressions**: Uses `Expression.Lambda` with `Compile()` — demonstrates LINQ Expressions code generation, and validates monitoring for indirect code generation.

These assemblies are marked with obvious names (`zTest*`) to prevent accidental inclusion in production scenarios. They are test-only and should never be deployed.

## Verification Steps

### 1. Build Verification
```powershell
dotnet build
# Expected: Build successful, all projects compile with 0 warnings
```

### 2. Test Execution
```powershell
dotnet test --logger "console;verbosity=detailed"
# Expected: 204 tests total (202 passing, 2 skipped by design — see the
# Skip attributes in SecurityViolationTests.cs for why)
```

### 3. Specific Feature Tests
```powershell
# Test DisallowDynamicAssemblies
dotnet test --filter "ClassName=DisallowDynamicAssembliesTests"
# Expected: 12 tests passing

# Test GlobalDynamicAssemblyMonitoring  
dotnet test --filter "ClassName=GlobalDynamicAssemblyMonitoringTests"
# Expected: 10 tests passing
```

### 4. Coverage Report
```powershell
dotnet test --collect:"XPlat Code Coverage"
```
Generates a Cobertura report per test run; no specific coverage threshold is
currently tracked or enforced in CI.

## Troubleshooting

### Tests Timing Out
**Issue**: Test takes longer than expected to complete

**Solution**: 
- Weak reference cleanup may be slow if GC hasn't run
- Consider running with GC.Collect() before assertions
- Check system resources

### Memory Leak Detected
**Issue**: Test reports memory usage increasing

**Solution**:
- Dispose contexts properly in finally blocks
- Force GC.WaitForPendingFinalizers() after dispose
- Verify weak references are being cleaned

### Event Not Raised
**Issue**: SecurityViolation event not firing

**Cause**: Check these in order
1. Policy has DisallowDynamicAssemblies = true?
2. EnableGlobalDynamicAssemblyMonitoring() was called?
3. Dynamic assembly actually created (not loaded)?
4. Sufficient time for event handler execution?

**Solution**:
```csharp
// Verify policy
Assert.True(context.SecurityPolicy.DisallowDynamicAssemblies);

// Verify subscription
context.EnableGlobalDynamicAssemblyMonitoring();

// Verify wait time
Thread.Sleep(100); // Give event handler time to run

// Verify dynamic assembly
var asm = AssemblyBuilder.DefineDynamicAssembly(...);
Assert.True(asm.IsDynamic); // Confirm it's dynamic
```

## Best Practices

### Writing New Tests
1. **Arrange-Act-Assert**: Clear test structure
2. **Resource Cleanup**: Use try/finally or using statements
3. **Meaningful Names**: Test name describes what's verified
4. **Isolated Tests**: Don't depend on execution order
5. **Clear Assertions**: Specific error messages

### Test Assembly Usage
1. **zTestRiskyAssembly**: Use for Reflection.Emit scenarios
2. **zTestLinqExpressions**: Use for expression compilation scenarios
3. **Never Deploy**: These are test-only assemblies
4. **Document Intent**: Comments explaining why assembly is risky

## Integration with CI/CD

### GitHub Actions Example
```yaml
- name: Run Security Tests
  run: |
    dotnet test src/Xcaciv.LoaderTests/Xcaciv.LoaderTests.csproj \
      --filter "ClassName=DisallowDynamicAssembliesTests or ClassName=GlobalDynamicAssemblyMonitoringTests" \
      --logger "trx" \
      --collect:"XPlat Code Coverage"
```

## Documentation References

- **[security-features-v2.md](security-features-v2.md)** — the security features these tests exercise
- **[CHANGELOG.md](../CHANGELOG.md)** — version history and when these features shipped

## Support

For issues or questions:
1. Check test names for hints about specific features
2. Review test implementation in test files
3. Examine test assemblies (zTestRisky*, zTestLinq*)
4. Run tests with verbose logging
5. Check security event messages

## Version Compatibility

- **Framework**: .NET 8.0+ (opt in to .NET 10.0 with `/p:UseNet10=true`; see [multi-framework.md](multi-framework.md))
- **Testing Framework**: xUnit
- **Supported Platforms**: Windows, Linux, macOS
- **Build Tool**: dotnet CLI

## Future Testing Considerations

- [ ] Performance benchmarking tests
- [ ] Stress testing with many dynamic assemblies created rapidly
- [ ] Memory profiling to verify weak references are properly cleaned
- [ ] Cross-domain testing (if applicable)
- [ ] Snapshot testing for policy configurations
