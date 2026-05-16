# ProjectMarworyn

A population simulation console application inspired by games such as Dwarf Fortress, Banished, Stardew Valley, and Crusader Kings 2. Starting from a seeded initial population, the simulation runs generation-by-generation — pairing individuals, producing children with blended names, and continuing until the population falls below two people.

## License

This project is licensed under the PolyForm Noncommercial License 1.0.0 - see the [LICENSE.md](LICENSE.md) file for details.

## Features

- **Deterministic world seed** — three random words are selected from a seed-word list and hashed (SHA-256) into a single integer seed, making every run reproducible by seed.
- **Name inheritance** — children receive blended names built from their parents' prefixes and suffixes.
- **Generational loop** — each generation pairs individuals, produces 0–3 children per pair, and passes the survivors to the next generation.
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
| Testing | xUnit + NSubstitute + coverlet |

## Code Standards

### General

- **Namespace:** All classes use the `ProjectMarworyn` namespace
- **Nullable reference types:** Disabled (`<Nullable>disable</Nullable>`)
- **Async/await:** Use async/await throughout
- **var:** Prefer `var` for local variable declarations
- **Trailing whitespace:** Lines must not end with trailing whitespace; `.cs` files must not end with an empty newline
- **Git branches:** Default branch is `main`; update branches are named `Update/Version[num]_[Mon][yy]`

### Parameter Formatting
When method calls contain multiple parameters, each parameter should be placed on a new line with one level of indentation:

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

### Naming Conventions

| Element | Convention | Example |
|---|---|---|
| Interfaces | Prefix with `I` | `IFileManager` |
| Models | Plain nouns | `Name`, `Pair` |
| DI extension methods | Prefix with `Add` | `AddServices` |
| Lists / collections | Plural names | `names`, `people` |
| Classes | Meaningful suffixes encouraged | `GenerationManager`, `ConsoleService` |

Methods, properties, and variables should be self-documenting through clear, descriptive names. Clarity over brevity.

### File Organisation

- One class per file
- File name must match class name exactly
- Group related classes in folders (`Models/`, `Extensions/`, `Services/`, etc.)
- Public methods before private methods — this takes precedence over all other ordering
- Within each visibility group, order methods roughly by calling sequence

### Architecture

- **Dependency injection:** All services registered through the DI container; registrations live in `Extensions/ServiceExtensions.cs`
- **Service lifetimes:** Transient for stateless services, Singleton for application-wide state
- **Separation of concerns:** Do not mix file I/O with business logic
- **Interface segregation:** Define services with interfaces

### Comments

- Prefer self-documenting code through clear naming
- Add a comment only when the *why* is non-obvious: a hidden constraint, a workaround, a subtle invariant
- Do not use XML doc comments, summaries, or `#region` blocks
- Comments describe *why*, never *what*

### Changelog Format

Entries in `CHANGELOG.md` use a hyphen-as-prefix style with no space after the hyphen. This is an intentional stylistic choice — it avoids the excessive whitespace Markdown renderers add to standard bullet lists and gives the changelog a retro aesthetic consistent with the project's theme.

- `-Main entry` — top-level change
- `+Sub-point` — detail or side effect nested under the entry above

Example:
```
-Name.json is now InitialPeople.json
+As a side effect of the above: DeathEngine tuple has been removed
```

### What Not To Do

- Don't hardcode file paths; use relative paths from the application directory
- Don't bypass dependency injection
- Don't create `new Random()` instances inside loops
- Don't use `#region`; split large files into smaller ones instead
- Don't add trailing whitespace to lines or to the end of files

For AI agent-specific instructions, see [AGENTS.md](AGENTS.md).

## Project Structure

```
ProjectMarworyn/
├── src/
│   └── ProjectMarworyn/
│       ├── Configuration/
│       │   ├── AppSettings.cs        # Strongly-typed settings
│       │   ├── InitialPeople.json             # Initial population data (deserialises to InitialPerson)
│       │   └── SeedWord.json         # Word pool for world seed generation
│       ├── Extensions/
│       │   └── ServiceExtensions.cs  # DI registrations
│       ├── Generators/
│       │   ├── DiceGenerator.cs      # Seeded Random factory
│       │   ├── IDiceGenerator.cs
│       │   ├── PersonGenerator.cs    # Creates Person instances from initial data and birth events
│       │   ├── IPersonGenerator.cs
│       │   ├── SeedGenerator.cs      # World seed creation
│       │   └── ISeedGenerator.cs
│       ├── Models/
│       │   ├── Enums/
│       │   │   ├── DeathModifier.cs  # Age-bracket death probability modifiers
│       │   │   └── Gender.cs         # Female / Male enum
│       │   ├── Generation.cs         # Iteration + person list
│       │   ├── InitialPerson.cs      # FullName, Prefix, Suffix, Gender — maps from InitialPeople.json
│       │   ├── Name.cs               # FullName, Prefix, Suffix
│       │   ├── Pair.cs               # Matched pair of individuals
│       │   ├── Person.cs             # Individual with age, gender, name, and simulation state
│       │   ├── SimulationClock.cs    # In-world time state
│       │   └── SeedWord.cs           # Id + Word
│       ├── Services/
│       │   ├── IConsoleService.cs
│       │   └── ConsoleService.cs     # Console output wrapper
│       ├── AgeProcessor.cs           # Advances person age each tick
│       ├── IAgeProcessor.cs
│       ├── DeathEngine.cs            # Calculates and applies death outcomes
│       ├── IDeathEngine.cs
│       ├── FileManager.cs            # Reads InitialPeople.json & SeedWord.json
│       ├── IFileManager.cs
│       ├── GenerationManager.cs      # Manages generation state and extinction checks
│       ├── IGenerationManager.cs
│       ├── Heartbeat.cs              # Drives the simulation clock
│       ├── IHeartbeat.cs
│       ├── PairingEngine.cs          # Pairs eligible individuals each generation
│       ├── IPairingEngine.cs
│       ├── SimulationManager.cs      # Orchestrates the simulation loop
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
    "InitialPeopleFilePath": "Configuration/InitialPeople.json",
    "SeedWordFilePath": "Configuration/SeedWord.json"
  }
}
```

| Setting | Description |
|---|---|
| `Delay` | Milliseconds to pause between console outputs |
| `InitialPeopleFilePath` | Relative path to the initial population JSON file |
| `SeedWordFilePath` | Relative path to the seed-word pool JSON file |

### Initial population file format (`InitialPeople.json`)

Deserialises into `InitialPerson`. Each entry defines one member of the starting population.

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

1. **Load** — the initial population is read from `InitialPeople.json` and each entry is created as a `Person` with a randomly assigned age.
2. **Seed** — three words are drawn randomly from `SeedWord.json` and hashed to produce the world seed.
3. **Loop** — while more than one person remains:
   - The simulation clock ticks, advancing in-world time by one day.
   - Ages are updated and death is evaluated for each person based on their age bracket.
   - Eligible individuals are paired using the seeded random number generator.
   - Each pair may produce a child. A child's name is a blend of both parents' prefixes and suffixes, and their gender is assigned at birth.
   - Survivors are passed into the next iteration.
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

This architecture allows future features like birth/death events, aging, and seasonal changes to occur based on the simulation clock.

## Testing

Project Marworyn follows a pragmatic, behaviour-focused testing approach. Tests exist to verify that the software works correctly, they do not exist to hit coverage targets or satisfy process for its own sake.

### Principles

- Tests must be falsifiable. A test that cannot fail provides no value.
+ An example of a meaningless test is a test that tests a property getter and setter. This is testing that a core component of C# is functional which is not the purpose of this test suite.
- No enforced coverage percentage. Coverage is a tool, not a goal.
- Do not test external framework or external library code. Test your behaviour, not Microsoft's.
- Tests may be written before or after implementation. What matters is that they exist and are meaningful.
- A new bug fix should ideally be covered by a new unit test.
- Most importantly, this is critical, a unit test does NOT exist to test implementation details.
- Tests that fail every single time a minor refactor occurs even if the behaviour is unchanged are bad brittle tests.
- Tests must pass or fail reliably, a flakey test is a bad test.

### What is a "Unit"

A unit is a discrete **unit of behaviour**, not necessarily a single method or class. A unit test may cover a single method, a complex calculation, or a chain of methods. A **unit of behaviour** refers to whatever represents a coherent, testable isolated piece of behaviour. Crucially a **unit of behaviour** is divorced from a **unit of implementation**.

### Framework

xUnit + NSubstitute

### Naming Convention

Tests follow the pattern:

```
Action_Condition_ExpectedResult()
```

For example:
```
GetAge_BornToday_Returns0()
GetAge_LeapYearBirthday_ReturnsCorrectAge()
GetAge_NegativeDate_ThrowsException()
```

A single method with multiple code paths should have multiple tests — one per distinct behaviour or edge case.

### Style

- Arrange/Act/Assert structure is encouraged but not mandated
- No Should-style naming
- Test names should read as a description of behaviour, not an implementation detail
