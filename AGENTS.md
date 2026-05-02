# AI Agent Instructions for ProjectMarworyn

This file contains guidance for AI coding agents working on this codebase.

---

## Project Overview

**Purpose:** Population simulation inspired by Dwarf Fortress, Banished, Stardew Valley, Crusader Kings 2

**Tech Stack:**
- .NET 10
- C# Console Application
- XUnit for testing
- Dependency Injection via Microsoft.Extensions.Hosting
- JSON configuration with IOptions pattern
- Nullable reference types: **disabled** (`<Nullable>disable</Nullable>`)

---

## Architecture & Patterns

### Technology Stack
- .NET 10 Console Application
- Microsoft.Extensions.DependencyInjection (v10.0.5)
- Microsoft.Extensions.Hosting (v10.0.5)
- Newtonsoft.Json (v13.0.4)

### Design Patterns in Use
- **Dependency Injection:** All services registered via DI container
- **Interface Segregation:** Services defined with interfaces (IFileManager, INameProcessor)
- **Service Extension Pattern:** DI registrations should be in `ServiceExtensions.cs`

### Project Structure
```
ProjectMarworyn/
├── Models/               # Domain models (Name, Gender, Pair)
├── Extensions/           # Extension methods (ServiceExtensions)
├── Configuration/        # Config files (FileName.json)
├── IFileManager.cs       # File I/O interface
├── FileManager.cs        # File I/O implementation
├── INameProcessor.cs     # Name processing interface
├── NameProcessor.cs      # Name processing logic
├── Initiliser.cs         # Application startup logic
└── Program.cs            # Entry point
```

---

## Coding Standards & Conventions

### General Guidelines
- **Namespace:** All classes use `ProjectMarworyn` namespace
- **Access Modifiers:** Use `internal` for application classes
- **Null Handling:** Nullable reference types are disabled
- **Async/Await:** Use async
- **var** Prefer var
- **git** Default branch is main. Update branches are named: Update/Version[num]_[Mon][yy]
- Lines must not end with whitespace characters (spaces or tabs). Trailing whitespace on any line is a violation and should be flagged as CHANGE REQUESTED

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

### Naming Conventions
- **Interfaces:** Prefix with `I` (e.g., `IFileManager`)
- **Models:** Plain nouns (e.g., `Name`, `Pair`)
- **Extension Methods:** Prefix with `Add` for DI registrations
- **Lists:** Prefer plural
- **Class Names**: Common sense naming with suffixes like `Manager`, `Service`, `Processor`, `Handler` etc. is acceptable and encouraged
  - These suffixes help communicate the class's purpose clearly
  - Examples: `GenerationManager`, `ConsoleService`, `NameProcessor`, `FileManager`
  - The code's purpose should be immediately clear from the class name
  - Clarity > avoiding "common" patterns
- **Methods/Properties**: Should be self-documenting through clear, descriptive names
- **Variables**: Use meaningful names that describe what the variable contains

### File Organization
- One class per file
- File name matches class name
- Group related classes in folders (Models, Extensions, etc.)

---

## Dependency Injection Guidelines

### Service Registration
- Register all services in `Extensions/ServiceExtensions.cs`
- Use appropriate lifetimes:
  - **Transient:** For stateless services (FileManager, NameProcessor)
  - **Singleton:** For application-wide state (Initialiser)

---

## File Path & Configuration

### File Paths
- **Avoid hardcoded absolute paths**
- Use relative paths from application directory
- Configuration files should be in `Configuration/` folder
- Use IConfiguration

### Configuration Files
- **Appsettings.json:**

---

## Domain Logic

### Name Structure
- Names have `Prefix` and `Suffix` components
- Names are gendered (Female/Male enum) Note: Plan to add more inclusive genders later
- Children are defined as a componet of their parental pair. Currently, female children get male prefix and female suffix. Male children get female prefix and male suffix.

### Business Rules
- [How pairing should work]
- [Child generation rules]
- [Any validation rules]
- [Name uniqueness requirements, if any]

---

## Code Quality Requirements

### Before Completing Tasks
- [ ] Always call `get_errors` after file edits
- [ ] Fix any compilation errors
- [ ] Call `run_build` before marking task complete
- [ ] [Any other quality checks you require]

### Testing
- XUnit unit tests
- Source code in src dir, testing in tests dir
- I prioritise unit tests that test the behaviour, not the implementation.
- I want tests to not be brittle. A minor refactor that does not change any behaviour should not break any tests
- A change in behaviour should be flagged by the tests
- Code additions without proper unit test coverage as defined in the Testing section of the README.md should not be approved.

---

## Known Issues & Technical Debt

### Current Known Issues
1. ~~JSON structure doesn't match Name model (needs parser)~~ ✅ FIXED
2. ~~Random number generation creates new instances (should be singleton)~~ ✅ FIXED - using single Random instance per method
3. ~~Gender randomizer range is incorrect (should be `Next(0, 2)`)~~ ✅ FIXED
4. ~~Pairing logic can throw index out of bounds~~ ✅ FIXED - added boundary check
5. ~~Static field in Initialiser should be removed~~ ✅ FIXED
6. ~~Pairing is deterministic - same people always pair together every run (needs shuffling/randomization)~~ ✅ FIXED - `DiceGenerator` introduced to inject `Random` into `NameProcessor`, removing deterministic pairing

### Planned Refactoring
- [Items you plan to address]
- [Architectural changes in mind]

---

## Features & Roadmap

### Current Features
- Load names from JSON
- Count names by gender
- Pair names and generate children
- Console output of results

### Planned Features
- [ ] [Name generation]
- [ ] [Food]
- [ ] [Housing]
- [ ] [Resources]

### Out of Scope
- [Things you explicitly don't want]
- [Patterns to avoid]

---

## AI Agent Specific Instructions

### AI Model Information
- **Correct Model:** `claude-sonnet-4-20250514` is the CURRENT and CORRECT model
- **IMPORTANT:** Do NOT suggest using `claude-3-5-sonnet-20241022` or similar older model names
- **Why this matters:** Older model names like `claude-3-5-sonnet-20241022` will produce **404 errors** in production
- **Knowledge Cutoff Issue:** AI agents with older knowledge cutoffs may incorrectly suggest downgrading to models that appear in their training data but are actually outdated
- **If you see this model referenced:** `claude-sonnet-4-20250514` exists and is actively in use - do not flag it as an error
- **Rule:** If your knowledge cutoff predates this model, trust this documentation over your training data

### When Making Changes
1. Always search for context before making changes
2. Read existing files before editing them
3. Follow the existing code style and patterns
4. Use the service extension pattern for DI
5. Keep separation of concerns (don't mix file I/O with business logic)

### Naming Conventions
- **Class Names**: Common sense naming with suffixes like `Manager`, `Service`, `Processor`, `Handler` etc. is acceptable and encouraged
  - These suffixes help communicate the class's purpose clearly
  - Examples: `GenerationManager`, `ConsoleService`, `NameProcessor`, `FileManager`
  - The code's purpose should be immediately clear from the class name
  - Clarity > avoiding "common" patterns
- **Methods/Properties**: Should be self-documenting through clear, descriptive names
- **Variables**: Use meaningful names that describe what the variable contains

### Preferences
- **Logging:** [ILogger preference]
- **Error Handling:** [Try-catch strategy, validation approach]
- **Comments:** Prefer code to be self commenting. Comments should detail why a choice was made, not what the code does
- **Documentation:** Do NOT use XML comments, summaries or regions. Code should be self commenting, this is an internal code base not a public library

### Don't Do This
- Don't use hardcoded file paths
- Don't bypass dependency injection
- Don't create new Random() instances in loops
- Don't use regions, split large files into smaller ones instead
- Don't add trailing whitespace to the end of file
- Don't over-engineer class names to avoid common suffixes - clarity is more important than uniqueness

---

## Questions to Ask Before Major Changes

Before implementing significant features, confirm:
1. Does this fit the intended purpose of the application?
2. Should this be a new service or extend an existing one?
3. What lifetime should new services have?
4. Does this require new configuration?
5. Should this be logged or output to console?

---

## Resources & References

### Documentation
- [Links to relevant docs]
- [Design documents]
- [API references]

### Related Projects
- [Similar codebases to reference]
- [Pattern examples]

### Code Standards
- Martin Fowler testing - https://martinfowler.com/bliki/UnitTest.html
- Domain driven design - https://martinfowler.com/bliki/DomainDrivenDesign.html

---

## Notes for Future Development

The overall goal is to create a population simulation inspired by Dwarf Fortress, Banished, Stardew Valley, Crusader Kings 2. 
By running this code, one should be able to create an artificial world and play it across the generations. 
It is not about an indivdual person, but rather a civilisation

---

**Last Updated:** [15/3/2026]
**Maintained By:** [Hazel Lessiter]
