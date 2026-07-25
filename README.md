# ProjectMarworyn

A population simulation game built with MonoGame, inspired by games such as Dwarf Fortress, Banished, Stardew Valley, and Crusader Kings 2. Starting from a seeded initial population, the simulation runs day-by-day — pairing individuals, producing children with blended names, and continuing until the population falls below two people.

## License

This project is licensed under the PolyForm Noncommercial License 1.0.0 - see the [LICENSE.md](LICENSE.md) file for details.

### Fonts

This project utilises the unmodified Pixeloid font by GGBotNet. It can be found here: https://ggbot.itch.io/pixeloid-font See [License.txt](src/ProjectMarworyn/Content/Fonts/Pixeloid_Font_1_0/License.txt) for details.

## Features

- **Deterministic world seed** — three random words are selected from a seed-word list and hashed (SHA-256) into a single integer seed, making every run reproducible by seed.
- **Name inheritance** — children receive blended names built from their parents' prefixes and suffixes; non-binary children draw from three naming routes (traditional, prefix + prefix, or suffix + suffix).
- **Biosex and gender modelled separately** — biosex (female/male/intersex) and gender (female/male/non-binary) are independent fields, with intersex, trans, and non-binary representation driven by ONS census 2021 statistics through seeded, configurable probabilities.
- **Orientation-driven pairing** — each person has an orientation (heterosexual, homosexual, bisexual, pansexual, asexual, aromantic, aroace) rolled at birth against a census-weighted table, and pairs only form on mutual attraction to each other's gender. A configurable slice of the population never pairs at all, regardless of orientation.
- **Biological reproduction** — children need a pair covering both the egg and sperm sides, derived from biosex (with fertile intersex people reproducing in the direction of their gender). Same-sex and asexual pairs form households but don't conceive — a planned adoption system will let them raise children orphaned in the simulation; people never appear from nowhere.
- **Day-based simulation** — the simulation ticks forward one real-time interval per in-world day, with pairing and births evaluated each new year and generations advancing every 20 years.
- **Simulation clock** — a universal heartbeat system tracks simulation time independently from real-world time, with each tick advancing the simulation by one day.
- **Graphical output** — simulation events are rendered in a MonoGame window using a pixel sprite font.
- **Multinational initial population** — starting names drawn from Irish, Scottish, Portuguese, Spanish, French, Belgian, Dutch, Danish, Swedish, Norwegian, Icelandic, Greenlandic, Faroese, Finnish, German, Swiss, Italian, and other traditions.

## Tech Stack

| Concern | Technology |
|---|---|
| Runtime | .NET 10 |
| Application type | MonoGame (WindowsDX) |
| Game framework | `MonoGame.Framework.WindowsDX` 3.8 |
| Dependency injection | `Microsoft.Extensions.DependencyInjection` |
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
│   ├── ProjectMarworyn/              # MonoGame application (WinExe)
│   │   ├── Content/
│   │   │   ├── Fonts/Pixeloid_Font_1_0/  # Pixeloid TrueType fonts
│   │   │   ├── Images/Button.png
│   │   │   ├── Content.mgcb          # MonoGame content pipeline manifest
│   │   │   └── SpriteFont.spritefont # Sprite font definition
│   │   ├── Program.cs                # Entry point — creates and runs Simulation
│   │   └── Simulation.cs             # MonoGame Game class; owns the update/draw loop
│   └── ProjectMarworyn.Core/         # Class library — all simulation logic
│       ├── Configuration/
│       │   ├── AppSettings.cs        # Strongly-typed settings
│       │   ├── DeathBracket.cs       # Age bracket + daily death chance (config shape)
│       │   ├── OrientationWeight.cs  # Orientation + weight in % (config shape)
│       │   ├── SimulationConstants.cs # Fixed simulation constants (days per year)
│       │   ├── InitialPeople.json    # Initial population data (deserialises to InitialPerson)
│       │   └── SeedWord.json         # Word pool for world seed generation
│       ├── Extensions/
│       │   └── ServiceExtensions.cs  # DI registrations (AddCoreServices)
│       ├── Generators/
│       │   ├── DiceGenerator.cs      # Seeded Random factory
│       │   ├── IDiceGenerator.cs
│       │   ├── PersonGenerator.cs    # Creates Person instances from initial data and birth events
│       │   ├── IPersonGenerator.cs
│       │   ├── SeedGenerator.cs      # World seed creation
│       │   └── ISeedGenerator.cs
│       ├── Managers/
│       │   ├── FileManager.cs        # Reads InitialPeople.json & SeedWord.json
│       │   ├── IFileManager.cs
│       │   ├── GenerationManager.cs  # Extinction checks
│       │   ├── IGenerationManager.cs
│       │   ├── SimulationManager.cs  # Orchestrates the day-by-day simulation loop
│       │   └── ISimulationManager.cs
│       ├── Models/
│       │   ├── Enums/
│       │   │   ├── Biosex.cs         # Female / Male / Intersex enum
│       │   │   ├── BiosexModifier.cs
│       │   │   ├── Gender.cs         # Female / Male / NonBinary enum
│       │   │   └── Orientation.cs    # Heterosexual through Aroace enum
│       │   ├── ChildGenerationResult.cs # Children + updated people from GenerateChildren
│       │   ├── GameState.cs          # Shared state — text lines rendered each frame
│       │   ├── InitialPerson.cs      # FullName, Prefix, Suffix, Biosex, Gender, Orientation, WillPair, IsFertile — maps from InitialPeople.json
│       │   ├── Name.cs               # FullName, Prefix, Suffix
│       │   ├── Pair.cs               # Matched pair of individuals
│       │   ├── PairingResult.cs      # Pairs + updated people from GeneratePairs
│       │   ├── Person.cs             # Individual with age, biosex, gender, name, and simulation state
│       │   ├── SimulationClock.cs    # In-world time state
│       │   └── SeedWord.cs           # Id + Word
│       ├── Appsettings.json          # Runtime settings (see Configuration below)
│       ├── AgeProcessor.cs           # Advances person age each tick
│       ├── IAgeProcessor.cs
│       ├── AttractionCalculator.cs   # Orientation × gender attraction policy
│       ├── IAttractionCalculator.cs
│       ├── DeathEngine.cs            # Calculates and applies death outcomes
│       ├── IDeathEngine.cs
│       ├── Heartbeat.cs              # Drives the simulation clock
│       ├── IHeartbeat.cs
│       ├── PairingEngine.cs          # Pairs mutually attracted individuals
│       └── IPairingEngine.cs
└── tests/
    ├── ProjectMarworyn.UnitTests/    # xUnit unit test project
    └── ProjectMarworyn.IntegrationTests/  # xUnit integration test project
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Windows (MonoGame WindowsDX target)

### Running the application

```bash
cd src/ProjectMarworyn
dotnet run
```

The first build restores the MonoGame content pipeline tools automatically — this may take a moment on a fresh clone.

### Running the tests

```bash
dotnet test
```

## Configuration

Settings are read from `src/ProjectMarworyn.Core/Appsettings.json` under the `Configuration` section.

```json
{
  "Configuration": {
    "InitialPeopleFilePath": "Configuration/InitialPeople.json",
    "SeedWordFilePath": "Configuration/SeedWord.json",
    "DayDuration": "0.00:00:0.50",
    "FertilityCooldownYears": 2,
    "TransgenderProbability": 0.2,
    "NonBinaryProbability": 0.06,
    "OrientationWeights": [
      { "Orientation": "Heterosexual", "Weight": 96.81 },
      { "Orientation": "Aroace", "Weight": 0.05 }
    ],
    "NeverPairProbability": 1,
    "IntersexFertileProbability": 50,
    "DeathBrackets": [
      { "MaxAge": 9, "DailyDeathChance": 0.1 },
      { "MaxAge": 99, "DailyDeathChance": 1.0 },
      { "DailyDeathChance": 2.5 }
    ]
  }
}
```

The `DeathBrackets` and `OrientationWeights` samples above are abridged — the real file has one bracket per decade of life and one weight per orientation.

| Setting | Description |
|---|---|
| `DayDuration` | Real-time duration of one in-world day (TimeSpan format, e.g. `"0.00:00:0.50"` for 500 ms) |
| `InitialPeopleFilePath` | Relative path to the initial population JSON file |
| `SeedWordFilePath` | Relative path to the seed-word pool JSON file |
| `FertilityCooldownYears` | Elapsed years before a parent can have another child (cooldown years are a fixed 365 days) |
| `TransgenderProbability` | Chance in % that a child's gender is the binary opposite of their biosex-aligned gender (default 0.2, ONS census 2021: trans men 0.10% + trans women 0.10%) |
| `NonBinaryProbability` | Chance in % that a child is non-binary, rolled independently of `TransgenderProbability` and taking precedence (default 0.06, ONS census 2021) |
| `OrientationWeights` | One weighted entry per `Orientation` value, in %, summing to 100 — newborns roll their orientation against the cumulative bands (defaults from ONS census 2021; the aromantic/aroace weights are placeholders, no census records them) |
| `NeverPairProbability` | Chance in % that a newborn never pairs regardless of orientation (`WillPair = false`, default 1) |
| `IntersexFertileProbability` | Chance in % that an intersex newborn is fertile (default 50 — a modelling estimate, no citable figure exists); binary biosex is always fertile |
| `DeathBrackets` | Ordered age brackets with a daily death chance in % — the first bracket whose `MaxAge` fits wins; omit `MaxAge` on the final bracket to make it the catch-all |

### Initial population file format (`InitialPeople.json`)

Deserialises into `InitialPerson`. Each entry defines one member of the starting population.

```json
[
  { "FullName": "Alys", "Prefix": "A", "Suffix": "lys", "Biosex": 0, "Gender": 0 },
  { "FullName": "Carys", "Prefix": "Ca", "Suffix": "rys", "Biosex": 0, "Gender": 0, "Orientation": 1 },
  { "FullName": "Hedda", "Prefix": "Hed", "Suffix": "da", "Biosex": 0, "Gender": 0, "WillPair": false }
]
```

`Biosex` values: `0` = Female, `1` = Male, `2` = Intersex.
`Gender` values: `0` = Female, `1` = Male, `2` = NonBinary — explicit per person, not derived from biosex, so trans and non-binary people can be seeded directly.
`Orientation` values: `0` = Heterosexual, `1` = Homosexual, `2` = Bisexual, `3` = Pansexual, `4` = Asexual, `5` = Aromantic, `6` = Aroace — defaults to Heterosexual when omitted.
`WillPair` and `IsFertile` default to `true` — only the exceptions (never-pairing or infertile people) carry them in the data file.

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
3. **Loop** — the MonoGame update loop calls `SimulationManager.ProgressDay()` once per `DayDuration` interval:
   - The simulation clock ticks, advancing in-world time by one day.
   - Each person ages up when the calendar reaches their birthday (leap years included; leap-day babies age up on 1 March in non-leap years), and death is evaluated based on their age bracket.
   - Each day, single adults who are willing to pair look for a mutually attractive partner — attraction is decided by each side's orientation against the other's gender — and pair off via seeded rolls.
   - Pairs covering both biological roles (one egg side, one sperm side, derived from biosex) can conceive. Newborns are assigned a biosex and, independently, a gender, orientation, pairing willingness, and (for intersex children) fertility via seeded rolls against the configured probabilities, then receive a blended name built from both parents' prefixes and suffixes.
   - On 1 January each year, population statistics are logged to the screen. Every 20 years the generation counter increments.
   - The current date, population count, and events are written to `GameState.Text` and rendered to the window each frame.
4. **End** — when fewer than two individuals remain, extinction is declared and the clock stops.

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

### Test projects

| Project | Purpose |
|---|---|
| `ProjectMarworyn.UnitTests` | Unit tests for individual behaviours |
| `ProjectMarworyn.IntegrationTests` | End-to-end tests covering the full simulation pipeline, seed generation, and population flow |

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
