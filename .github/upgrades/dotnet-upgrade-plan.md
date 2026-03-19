# .NET 10 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 10 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10 upgrade.
3. Upgrade src\ProjectMarworyn\ProjectMarworyn.csproj
4. Upgrade tests\ProjectMarworyn.Tests\ProjectMarworyn.Tests.csproj
5. Run unit tests to validate upgrade in the projects listed below:
   - tests\ProjectMarworyn.Tests\ProjectMarworyn.Tests.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

None.

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                                          | Current Version | New Version | Description                            |
|:------------------------------------------------------|:---------------:|:-----------:|:---------------------------------------|
| Microsoft.Extensions.DependencyInjection              |   9.0.8         |  10.0.5     | Recommended for .NET 10                |
| Microsoft.Extensions.DependencyInjection.Abstractions |   9.0.8         |  10.0.5     | Recommended for .NET 10                |
| Microsoft.Extensions.Hosting                          |   9.0.8         |  10.0.5     | Recommended for .NET 10                |
| Microsoft.Extensions.Options                          |   9.0.8         |  10.0.5     | Recommended for .NET 10                |
| Newtonsoft.Json                                       |   13.0.3        |  13.0.4     | Recommended patch update               |

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### src\ProjectMarworyn\ProjectMarworyn.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.DependencyInjection should be updated from `9.0.8` to `10.0.5` (*recommended for .NET 10*)
  - Microsoft.Extensions.DependencyInjection.Abstractions should be updated from `9.0.8` to `10.0.5` (*recommended for .NET 10*)
  - Microsoft.Extensions.Hosting should be updated from `9.0.8` to `10.0.5` (*recommended for .NET 10*)
  - Microsoft.Extensions.Options should be updated from `9.0.8` to `10.0.5` (*recommended for .NET 10*)
  - Newtonsoft.Json should be updated from `13.0.3` to `13.0.4` (*recommended patch update*)

#### tests\ProjectMarworyn.Tests\ProjectMarworyn.Tests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`
