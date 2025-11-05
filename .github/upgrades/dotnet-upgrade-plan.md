# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade src\zTestInterfaces\zTestInterfaces.csproj
4. Upgrade src\Xcaciv.Loader\Xcaciv.Loader.csproj
5. Upgrade src\Xcaciv.LoaderTests\Xcaciv.LoaderTests.csproj
6. Upgrade src\zTestDependentAssembly\zTestDependentAssembly.csproj
7. Upgrade src\TestAssembly\zTestAssembly.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

Table below contains projects that do belong to the dependency graph for selected projects and should not be included in the upgrade.

| Project name                                   | Description                 |
|:-----------------------------------------------|:---------------------------:|

### Aggregate NuGet packages modifications across all projects

No NuGet package modifications were identified by analysis.

### Project upgrade details
This section contains details about each project upgrade and modifications that need to be done in the project.

#### src\zTestInterfaces\zTestInterfaces.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - None detected.

Other changes:
  - No code changes identified by analysis; only target framework update is required.

#### src\Xcaciv.Loader\Xcaciv.Loader.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - None detected.

Other changes:
  - After changing target framework, run a build and fix any compilation issues and API breaking changes if they appear.

#### src\Xcaciv.LoaderTests\Xcaciv.LoaderTests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - None detected.

Other changes:
  - Update test SDK or packages if they report compatibility issues after upgrade.

#### src\zTestDependentAssembly\zTestDependentAssembly.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - None detected.

Other changes:
  - This project references `Fastenshtein` library in code. Ensure the package used by this project is compatible with .NET 10.0; update package if build reports issues.

#### src\TestAssembly\zTestAssembly.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - None detected.

Other changes:
  - No code changes identified by analysis; only target framework update is required.
