# .NET 10.0 Upgrade Report

## Project target framework modifications

| Project name                                   | Old Target Framework    | New Target Framework         | Commits                   |
|:-----------------------------------------------|:-----------------------:|:----------------------------:|---------------------------|
| src\zTestInterfaces\zTestInterfaces.csproj   |   net8.0               | net10.0                     | 75c7fc2c                  |
| src\Xcaciv.Loader\Xcaciv.Loader.csproj      |   net8.0               | net10.0                     | 853306da                  |
| src\Xcaciv.LoaderTests\Xcaciv.LoaderTests.csproj | net8.0             | net10.0                     | 853306da                  |
| src\zTestDependentAssembly\zTestDependentAssembly.csproj | net8.0     | net10.0                     | 853306da                  |
| src\TestAssembly\zTestAssembly.csproj       |   net8.0               | net10.0                     | 853306da                  |

## NuGet Packages

No NuGet package updates were made.

## All commits

| Commit ID              | Description                                |
|:-----------------------|:-------------------------------------------|
| 75c7fc2c               | Commit upgrade plan                         |
| 853306da               | Update projects to target .NET 10.0         |

## Project feature upgrades

No feature upgrades were necessary beyond target framework changes.

## Test results

- Test project `Xcaciv.LoaderTests` ran: Passed 4, Failed 5, Skipped 0.

## Next steps

- Investigate failing tests in `Xcaciv.LoaderTests` and address compatibility or behavioral changes introduced by the upgrade.