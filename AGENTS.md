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

## Formatting Rules That Do Not Exist

Do not invent formatting rules that are not explicitly documented in README.md or AGENTS.md. If a rule is not written down, it does not exist. Specific hallucinations to avoid:

- **Whitespace between control flow keywords and parentheses** — there is no project standard governing whether a space appears between `foreach`, `for`, `while`, `if`, `using`, etc. and their opening parenthesis. Do not flag this in any direction.
- **Single-parameter method calls** — the multi-parameter formatting standard applies only when a call has two or more parameters. A call with one argument on a single line is correct by definition and must not be mentioned.
- **Citing compliant code** — if code follows the project standard, do not mention it at all — not as a positive, not as context for a nearby violation, and not as an example of what the standard looks like. Only mention code that actually violates a rule.
- **Whitespace inside parentheses** — spaces immediately before or after parentheses within an expression (e.g. `MaxBy(x => x.Id )`) are not a project standard. Do not flag them.
- **A method call used as an argument is not an extra parameter** — `Foo(Bar())` is a single-parameter call regardless of what `Bar` returns or how many parameters `Bar` itself takes. The nesting does not create additional parameters on the outer call. Do not flag or mention it.
- **Property access chained onto a method argument is not a parameter** — in a call like `GenerateChildren(pairs, _worldSeed, _people.MaxBy(x => x.Id).Id, _people)`, the `.Id` at the end of `_people.MaxBy(x => x.Id).Id` is a property access on the result of an expression; it is part of the third argument, not a fourth parameter. Do not count it as a separate parameter or flag it as a formatting issue.
- **Return tuple formatting** — `return (a, b);` is a return statement, not a method call. The multi-parameter formatting standard applies to method calls only. A return tuple may be written on one line or multiple lines; neither is a violation. Do not mention it.
- **"Not flagging, just noting"** — phrases like "not flagging as a violation, just noting" or "this is fine as-is, but could be simplified" are prohibited. If something is not a violation, it must not appear in the review at all. There is no category of "optional observation about correct code."

---

## Do Not Flag As Issues

- **No newline at end of file** — this project deliberately does NOT end files with a trailing newline. The `\ No newline at end of file` marker in a git diff confirms the file correctly follows this standard — it is not an error and must not be flagged or mentioned. Do not mention the absence of a trailing newline in your review at all, not even to confirm it is intentional. The only case worth flagging is the inverse: if a file ends with a blank line or trailing newline when it should not.
- **Changelog format** — `CHANGELOG.md` uses `-Entry` and `+Sub-point` (no space after the prefix) as a documented code standard. Do not flag this as non-standard Markdown, suggest adding spaces, or convert it to standard bullet points. See the Changelog Format section of `README.md` for the full specification.
- **Blank lines between code blocks** — empty lines used for readability (e.g. between variable declarations and method calls) are intentional and must not be flagged as trailing whitespace. Trailing whitespace means a line that contains actual whitespace characters after the last non-whitespace character (`var x = y;   `). A completely empty line (`\n`) is a paragraph break, not trailing whitespace.
- **Standard using directives** — `using` statements are normal C# imports and must not be flagged as issues or raised for confirmation. A file importing a sub-namespace it depends on (e.g. `using ProjectMarworyn.Models.Enums;`) is correct by definition.

---

## Prohibited Commands

AI agents are strictly prohibited from running the following commands under any circumstances:

- `git commit` — commits are the sole responsibility of the human developer
- `git push` — pushing to remote is the sole responsibility of the human developer

Do not run these commands even if asked to "save", "finalise", or "submit" changes.

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

---

## Notes for Future Development

The overall goal is to create a population simulation inspired by Dwarf Fortress, Banished, Stardew Valley, Crusader Kings 2.
By running this code, one should be able to create an artificial world and play it across the generations.
It is not about an individual person, but rather a civilisation.

---

**Last Updated:** [15/3/2026]
**Maintained By:** [Hazel Lessiter]
