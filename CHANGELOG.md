# Changelog

---

## [2.2.0]

-Pairing rewritten around mutual attraction (Issue #15 Stage 4): new `AttractionRules` decides attraction from orientation and the candidate's gender - heterosexual means any gender different from your own (so a heterosexual non-binary person is attracted to both binary genders), homosexual the same gender, bisexual the binary genders, pansexual everyone, asexual anyone; both sides must be attracted or no pair forms
+Aromantic and aroace people never pair; `WillPair` is respected as the orientation-independent opt-out
+Intersex people now enter the pairing pool - the old female/male biosex pools silently excluded them
-`Pair` is now `PersonA`/`PersonB` instead of `FPerson`/`MPerson`; `GenerateChildren` derives mother/father from biosex, so reproduction needs the egg and sperm sides covered and same-sex pairs don't conceive
+Adoption (later) will redistribute children orphaned in the simulation instead - people never appear from nowhere
-Asexual people pair romantically but never conceive - a child means sexual reproduction by definition until the adoption system exists
-Added `IsFertile` to `Person` and `IntersexFertileProbability` to `AppSettings` (in %, default 50 - a modelling estimate, no citable fertile/infertile split exists): intersex newborns roll it at birth, binary biosex is always fertile, and a fertile intersex person reproduces in the direction of their gender with non-binary gender able to fill either role
+`IsFertile` defaults to true in `InitialPeople.json` - only infertile people carry it in the data file
-`DeathEngine.ProcessDeaths` now returns the survivor list and the `Generation` model is removed - it was recreated every day just to carry `Iteration` plus the survivors (#41); `Iteration` now lives on `SimulationManager`

## [2.1.0]

-`Person.TimeLived` is replaced by an explicit birthday (`BirthMonth`/`BirthDay`) and `Person.TimeFromLastChild` by a plain day counter (`DaysSinceLastChild`) - no more 1-based `DateTime` fields misused as durations
+People age up when the simulation calendar hits their birthday, so leap years are modelled; those born on the 29th of February age up on the 1st of March in non-leap years
+`FertilityCooldownYears` now means true elapsed years: default changed from 3 to 2, preserving the same 730-day cooldown the old 1-based `DateTime.Year` comparison produced (cooldown years are a fixed 365 days via `SimulationConstants.DaysPerYear`)
+Fixed initial people never being born in December - the old month roll's upper bound was exclusive

-Fixed `GenerateChildren` conflating biosex and gender - a child's gender was a direct cast of their biosex
-Added `TransgenderProbability` to `AppSettings` (in %, default 0.2 = trans man 0.10% + trans woman 0.10% per ONS census 2021) - a child's gender now aligns with their biosex in the majority of cases, with a small seeded chance of deviating
-Added `Gender` to `InitialPerson` and `InitialPeople.json` - initial people's gender is now explicit data instead of being derived from biosex, at the same ~0.5% trans representation (Tammy)
-Fixed Björk's biosex in `InitialPeople.json` - was mistakenly male
-Added `NonBinary` to the `Gender` enum with `NonBinaryProbability` in `AppSettings` (in %, default 0.06 per ONS census 2021) - rolled independently of `TransgenderProbability`, with the non-binary roll taking precedence
-Seeded Raven as non-binary in `InitialPeople.json`
-Removed `AppSettings.Delay` - read by nothing since `DayDuration` took over pacing
-Replaced the `DeathModifier` enum with a configurable `DeathBrackets` table in `AppSettings` - age brackets with `DailyDeathChance` in %, same baseline values, no more basis points or bracket names doubling as values
+Also removed the dead `names` list in `DeathEngine`, left over from when a person was just a name
+`DeathEngine` validates at construction that the table has a catch-all bracket, so a hand-edited config fails at startup with a clear message instead of mid-run
-Fixed `NullReferenceException` in `SimulationManager.ProgressDay` when every remaining person dies on the same day - the extinction check runs at the start of the next day, so the rest of the day now guards against an empty population
-Intersex children now roll for trans like everyone else - their randomly assigned binary gender is the starting point the trans roll can flip (not visible today, but will be once trans status is tracked)
-Non-binary children now pick between three naming routes: traditional (either binary convention), prefix + prefix, or suffix + suffix, with either parent's part able to come first on the new routes - naming logic extracted into `PersonGenerator.CalculateName`
-Added `Orientation` to `Person` (Issue #15 Stage 3): heterosexual, homosexual, bisexual, pansexual, asexual, aromantic and aroace. Newborns roll it against a new `OrientationWeights` table in `AppSettings` (in %, defaults from ONS census 2021; aromantic and aroace at 0.05 each are invented placeholders - no census records them)
+`PersonGenerator` validates the table at construction (one entry per orientation, weights summing to 100), same pattern as the `DeathBrackets` guard
-Added `WillPair` to `Person` - a standalone pairing-willingness flag, independent of orientation and of `WillHaveChildren`; newborns roll it against `NeverPairProbability` in `AppSettings` (in %, default 1)
-Initial people carry `Orientation` and `WillPair` explicitly in `InitialPeople.json` (no dice at init, same as `Gender`): Carys and Gethin homosexual, Bonnie and Jordi bisexual, Hedda never pairs
+`InitialPerson.WillPair` defaults to true so only the exceptions appear in the data file
-Removed a redundant `IsAlive = true` reassignment in `DeathEngine` - the survivor branch only ever sees people already filtered alive (#39)

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