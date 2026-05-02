# ProjectMarworyn

A population simulation console application inspired by games such as Dwarf Fortress, Banished, Stardew Valley, and Crusader Kings 2. Starting from a seeded initial population, the simulation runs generation-by-generation — pairing individuals, producing children with blended names, and continuing until the population falls below two people.

## License
This project is licensed under the PolyForm Noncommercial License 1.0.0 - see the [LICENSE.md](LICENSE.md) file for details.

## Features

- **Deterministic world seed** — three random words are selected from a seed-word list and hashed (SHA-256) into a single integer seed, making every run reproducible by seed.
- **Name inheritance** — children receive blended names built from their parents' prefixes and suffixes.
- **Generational loop** — each generation pairs females and males, produces 0–3 children per pair, and passes the survivors to the next generation.
- **Simulation clock** — a universal heartbeat system tracks simulation time independently from real-world time, with each tick advancing the simulation by one day.
- **Configurable delay** — a millisecond delay between console outputs keeps the simulation readable.

## Tech Stack

| Concern | Technology |
|---|---|
| Runtime | .NET 10 |
| Application type | C# Console Application |
| Dependency injection | `Microsoft.Extensions.Hosting` / `Microsoft.Extensions.DependencyInjection` |
| Configuration | `IOptions<AppSettings>` + `Appsettings.json` |
| JSON deserialisation | `Newtonsoft.Json` |
| Testing | xUnit + coverlet |

## Code Standards

### Parameter Formatting
When method calls contain multiple parameters, each parameter should be placed on a new line with one level of indentation:

###csharp
### Parameter Formatting
- **Multi-parameter methods:** When a method call has multiple parameters, each parameter should be on a new line with one level of indentation
- **Example (Preferred):**
  ```csharp
  //Each new parameter after the first one on a new line and indented by one level
  var age = GetAge(person,
      timeLived);
  ```
- **Not:**
  ```csharp
  //All parameteres on one line
  var age = GetAge(person, timeLived);
  ```
  - **Not:**
  ```csharp
  //Parameters on new line, all parameters on one line, open bracket left on first line
  var age = GetAge(
    person, timeLived);
  ```
    - **Not:**
  ```csharp
  //All parameteres indented once with a new line, but no parameter on the first line, just an open bracket
  var age = GetAge(
    person,
    timeLived);
  ```
    - **Not:**
  ```csharp
  //First parameter on the first line, all other parameters with a new line and indented, but the indentation is alignment based
  var age = GetAge(person,
                   timeLived);
  ```
- **Rationale:** Improves readability, makes diffs clearer, and follows the project's established formatting convention
- **Note:** This applies to method calls with multiple parameters; single-parameter calls can remain on one line

This convention improves readability and makes version control diffs clearer. Single-parameter method calls may remain on one line.

For complete coding standards and AI agent instructions, see [AGENTS.md](AGENTS.md).

## Project Structure

```
ProjectMarworyn/
├── src/
│   └── ProjectMarworyn/
│       ├── Configuration/
│       │   ├── AppSettings.cs        # Strongly-typed settings
│       │   ├── Name.json             # Initial population data
│       │   └── SeedWord.json         # Word pool for world seed generation
│       ├── Extensions/
│       │   └── ServiceExtensions.cs  # DI registrations
│       ├── Models/
│       │   ├── Generation.cs         # Iteration + name list
│       │   ├── Gender.cs             # Female / Male enum
│       │   ├── Name.cs               # FullName, Prefix, Suffix, Gender
│       │   ├── Pair.cs               # Matched female + male
│       │   └── SeedWord.cs           # Id + Word
│       ├── Services/
│       │   ├── IConsoleService.cs
│       │   └── ConsoleService.cs     # Console output wrapper
│       ├── DiceGenerator.cs          # Seeded Random factory
│       ├── FileManager.cs            # Reads Name.json & SeedWord.json
│       ├── GenerationManager.cs      # Pairs names and produces children
│       ├── Initialiser.cs            # Application entry logic
│       ├── NameProcessor.cs          # Pairing and gender-count logic
│       ├── SeedGenerator.cs          # World seed creation
│       └── Program.cs                # Host setup and startup
└── tests/
    └── ProjectMarworyn.Tests/        # xUnit test project
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Running the application

```bash
cd src/ProjectMarworyn
dotnet run
```

### Running the tests

```bash
dotnet test
```

## Configuration

Settings are read from `src/ProjectMarworyn/Appsettings.json` under the `Configuration` section.

```json
{
  "Configuration": {
    "Delay": 500,
    "NameFilePath": "Configuration/Name.json",
    "SeedWordFilePath": "Configuration/SeedWord.json"
  }
}
```

| Setting | Description |
|---|---|
| `Delay` | Milliseconds to pause between console outputs |
| `NameFilePath` | Relative path to the initial population JSON file |
| `SeedWordFilePath` | Relative path to the seed-word pool JSON file |

### Name file format (`Name.json`)

```json
[
  { "FullName": "Alys", "Prefix": "A", "Suffix": "lys", "Gender": 0 }
]
```

`Gender` values: `0` = Female, `1` = Male.

### Seed-word file format (`SeedWord.json`)

```json
[
  { "id": 0, "word": "Acorn" }
]
```

Three words are chosen at random and combined (e.g. `ACORN-BIRCH-BROOK`) then SHA-256 hashed into the world seed integer.

## How the Simulation Works

1. **Load** — the initial population is read from `Name.json`.
2. **Seed** — three words are drawn randomly from `SeedWord.json` and hashed to produce the world seed.
3. **Loop** — while more than one person remains:
   - Female and male names are paired using the seeded random number generator.
   - Each pair produces 0–3 children. A child's name is a blend of both parents' names.
   - The new generation's names are passed into the next iteration.
4. **End** — when fewer than two individuals remain the simulation reports extinction and exits.

## Simulation Clock Architecture

The **Heartbeat** system provides a universal simulation clock that decouples in-world time from real-world time:

- **SimulationClock** (singleton) — holds the current simulation state:
  - `TickCount` — total number of ticks elapsed
  - `SimulationTime` — current in-world date/time
  - `StartTime` / `EndTime` — simulation boundaries
  - `ElapsedTime` — total in-world time passed
  - `IsRunning` — clock state

- **Heartbeat** — manages the clock:
  - `Start()` — begins the simulation at year 0001
  - `Tick()` — advances time by one day (configurable)
  - `Stop()` — pauses the simulation
  - `Reset()` — clears all state back to defaults

This architecture allows future features like birth/death events, aging, and seasonal changes to occur based on the simulation clock instead of generation boundaries.
