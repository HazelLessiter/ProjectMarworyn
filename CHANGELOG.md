# Changelog

---

## [Unreleased]

-Fixed `GenerateChildren` conflating biosex and gender - a child's gender was a direct cast of their biosex
-Added `TransgenderProbability` to `AppSettings` (in %, default 0.5 based on ONS census 2021) - a child's gender now aligns with their biosex in the majority of cases, with a small seeded chance of deviating
-Added `Gender` to `InitialPerson` and `InitialPeople.json` - initial people's gender is now explicit data instead of being derived from biosex, at the same ~0.5% trans representation (Tammy)
-Fixed Björk's biosex in `InitialPeople.json` - was mistakenly male

## [2.0.2]

-`PairingEngine.GeneratePairs` and `PersonGenerator.GenerateChildren` now return dedicated `PairingResult` and `ChildGenerationResult` types instead of tuples
-Added `FertilityCooldownYears` to `AppSettings` - previously a hardcoded `3` duplicated across `PersonGenerator` and `AgeProcessor`
-Fixed O(n²) performance issue in `PairingEngine` where the single male pool was rebuilt from scratch on every female instead of once

## [2.0.1]

-Fixed seed collision bug in `DiceGenerator` where different calendar dates could produce identical random sequences
-Rebalanced `DeathModifier` values back to realistic numbers
+Death/birth chance had been over-tuned while debugging an unrelated bug and was never brought back down afterwards
-`DeathEngine`, `PairingEngine` and `PersonGenerator` no longer share one `Random` instance passed down from `SimulationManager` - each now seeds its own internally from the world seed and current date
-Added `DiceGeneratorTests` - `DiceGenerator` previously had no dedicated test coverage
-Fixed dead, unused mock setup left behind in `PairingEngineTests`
-Removed the AI PR reviewer's parameter formatting rules entirely - it kept misapplying them and creating noise every review
-Added Known Issues and Pathway sections to `ROADMAP.md`
-Fixed a flaky integration test that relied on too small a sample size and could fail randomly

## [2.0.0]

-Added Monogame
-Fixed unit tests
-Fixed issue where dead pairs would accumulate forever
-Added GameState
-Added core simulation loop
-Fixed bug where there was an enumeration on a new Pair list instead of using the existing Pair list
-Added Irish, Scottish, Portugese, Spanish, French, Belgium, Dutch, Denmark, Swedish, Norweign, Icelandic, Greenland, Foreoese, Finnish, German, Swiss, Adorra, Monoco, Polish, Czech and Italian InitialNames
-Added integration tests
-Fixed issue where pairs weren’t persisted
-Fixed bug where parent's timeFromLastChild cooldown would be reset even on unsuccessful child generation attempt

## [1.1.0]

-Fixed issue where no one could die
-Claude PRs now apply to all branches, not just `main` and `Update/**`
-Removed Gender from Name, is now applied to Person only
-Name.json is now InitialPeople.json
+As a side effect of the above: DeathEngine tuple has been removed
-AIs are now prohibited from git push or git commit
-Added Intersex people - They currently don't pair
-Seperated gender and biosex - Children are now generated from biosex pairs

## [1.0.1] - 2nd May 2026

### Added

-Added PolyForm Non-Commercial Attribution licence

## [1.0.0] - 14th March 2026

### Added

-Life and Death of a Person is now triggered by the Heartbeat and SimulationClock not just a Generation loop
-People are now defined as a Person instead of just a Name
-Added PairingEngine
-Added Death
-Added SimulationClock
-Added CODEOWNERS
-Added `CHANGELOG.md` added to the repository root.
-Added `README.md` added to the repository root.
-World seed generation using three randomly selected words hashed via SHA-256 (`SeedGenerator`, `ISeedGenerator`).
-`DiceGenerator` / `IDiceGenerator` — seeded `Random` factory used across the simulation for reproducible results.
-Randomised pairing of female and male names each generation.
-Initial population simulation loop — pairs names by gender each generation and produces 0–3 children with blended names.
-`GenerationManager` — manages generation iterations and child creation.
-`NameProcessor` — handles gender-based name filtering and pairing.
-`FileManager` — reads `Name.json` and `SeedWord.json` configuration files.
-`ConsoleService` — wraps console output with configurable delay.
-`Initialiser` — application startup logic orchestrating the simulation loop.
-Dependency injection wired via `Microsoft.Extensions.Hosting` and `ServiceExtensions`.
-`Appsettings.json` configuration with `IOptions<AppSettings>` pattern for `Delay`, `NameFilePath`, and `SeedWordFilePath`.
-`Name.json` — initial population of Welsh-inspired names with prefix/suffix splits.
-`SeedWord.json` — pool of nature-themed words for world seed generation.
-xUnit test project with mocks for all core services.

### Fixed

-Fixed issue where AgeProcessor reset gender of all people to female
-Incorrect guard logic in `GenerateChildren` that caused premature generation termination.
-Seed word IDs corrected in `SeedWord.json`.
-Null guard added to `SeedGenerator.GetThreeWords()`.

### Refactored

-Improved exception handling in `FileManager` — `FileNotFoundException` and `JsonException` are now caught and rethrown with descriptive messages.

### Modified

-Updated to .NET 10.
-Unit tests updated to cover new seed and dice generator behaviour.
-`AGENTS.md` updated with model-naming guidance to avoid false errors.