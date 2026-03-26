# Changelog

---

## [1.0.0]

### Added
- Life and Death of a Person is now triggered by the Heartbeat and SimulationClock not just a Generation loop
- People are now defined as a Person instead of just a Name
- Added PairingEngine
- Added Death
- Added SimulationClock
- Added CODEOWNERS
- Added `CHANGELOG.md` added to the repository root.
- Added `README.md` added to the repository root.
- World seed generation using three randomly selected words hashed via SHA-256 (`SeedGenerator`, `ISeedGenerator`).
- `DiceGenerator` / `IDiceGenerator` — seeded `Random` factory used across the simulation for reproducible results.
- Randomised pairing of female and male names each generation.
- Initial population simulation loop — pairs names by gender each generation and produces 0–3 children with blended names.
- `GenerationManager` — manages generation iterations and child creation.
- `NameProcessor` — handles gender-based name filtering and pairing.
- `FileManager` — reads `Name.json` and `SeedWord.json` configuration files.
- `ConsoleService` — wraps console output with configurable delay.
- `Initialiser` — application startup logic orchestrating the simulation loop.
- Dependency injection wired via `Microsoft.Extensions.Hosting` and `ServiceExtensions`.
- `Appsettings.json` configuration with `IOptions<AppSettings>` pattern for `Delay`, `NameFilePath`, and `SeedWordFilePath`.
- `Name.json` — initial population of Welsh-inspired names with prefix/suffix splits.
- `SeedWord.json` — pool of nature-themed words for world seed generation.
- xUnit test project with mocks for all core services.

### Fixed
- Incorrect guard logic in `GenerateChildren` that caused premature generation termination.
- Seed word IDs corrected in `SeedWord.json`.
- Null guard added to `SeedGenerator.GetThreeWords()`.

### Refactored
- Improved exception handling in `FileManager` — `FileNotFoundException` and `JsonException` are now caught and rethrown with descriptive messages.

### Modified
- Updated to .NET 10.
- Unit tests updated to cover new seed and dice generator behaviour.
- `AGENTS.md` updated with model-naming guidance to avoid false errors.