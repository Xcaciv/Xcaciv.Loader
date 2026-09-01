# Xcaciv.Loader

[![Securability](https://localhost:3001/badge/10a3ad1d-30d9-43b6-8eb6-56a17d961323.svg)](https://localhost:3001/r/Xcaciv/Xcaciv.Loader)

Sexy simple C# module for runtime loading of types from external assemblies.

```csharp
    using (var context = new AssemblyContext(dllPath, basePathRestriction: AppDomain.CurrentDomain.BaseDirectory)) // Load
    {
        var myInstance = context.CreateInstance<IClass1>("Class1");
        return myInstance.Stuff("input here");
    } // Unload
```

## Features

- Dynamic assembly loading and unloading
- Type discovery and instantiation
- Security measures to prevent loading from restricted directories
- Automatic dependency resolution
- **Multi-framework support**: Build for both .NET 8.0 and .NET 10.0

## Multi-Framework Support

This library can be built for both .NET 8.0 (default) and .NET 10.0.

See the [Multi-Framework Documentation](docs/multi-framework.md) for details on how to build for different target frameworks.

## Documentation

For security features, migration guides, and the original design specification, see [docs/README.md](docs/README.md).
