# AI Agent Instructions for ProjectMarworyn

Full project documentation and coding standards are in [README.md](README.md). This file contains only AI-specific guidance that supplements it.

---

## Model

Use `claude-sonnet-4-6`. Do not suggest older model names such as `claude-sonnet-4-20250514` — these are deprecated and will produce errors.

---

## Before Completing Tasks

- [ ] Call `get_errors` after every file edit
- [ ] Fix any compilation errors before proceeding
- [ ] Call `run_build` before marking a task complete

---

## Do Not Flag As Issues

- **No newline at end of file** — this project deliberately does NOT end files with a trailing newline. Do not suggest adding one. If a file does end with a trailing newline, flag it for removal.
- **Blank lines between code blocks** — empty lines used for readability (e.g. between variable declarations and method calls) are intentional and must not be flagged as trailing whitespace. Trailing whitespace means a line that contains actual whitespace characters after the last non-whitespace character (`var x = y;   `). A completely empty line (`\n`) is a paragraph break, not trailing whitespace.

---

## When Making Changes

1. Search for existing context before writing new code
2. Read files before editing them
3. Follow the code style and patterns documented in README.md
4. Use the service extension pattern for DI registrations in `Extensions/ServiceExtensions.cs`
5. Keep separation of concerns — do not mix file I/O with business logic

---

## Testing

- Code additions without proper unit test coverage must be CHANGES REQUESTED, not minor suggestions — even if the code needs re-architecting to be unit testable
- Tests must test behaviour, not implementation — see the Testing section of README.md for the full philosophy
- Do not use XML comments, summaries, or regions anywhere in the codebase

---

## Questions to Ask Before Major Changes

1. Does this fit the intended purpose of the application?
2. Should this be a new service or extend an existing one?
3. What lifetime should new services have?
4. Does this require new configuration?
5. Should this be logged or output to console?

---

## Known Issues & Technical Debt

### Planned Refactoring
- Test project needs a proper mocking framework — do not use Moq (SponsorLink controversy)
- `ProcessDeaths` returns a tuple — would prefer a dedicated result type

---

## Notes for Future Development

The overall goal is to create a population simulation inspired by Dwarf Fortress, Banished, Stardew Valley, Crusader Kings 2.
By running this code, one should be able to create an artificial world and play it across the generations.
It is not about an individual person, but rather a civilisation.

---

**Last Updated:** [15/3/2026]
**Maintained By:** [Hazel Lessiter]