# Roadmap

## A scratchpad of where I want this project to go in the future

## WARNING - SUBJECT TO CHANGE!!!

## Fixes

Fix current issues (17-21)
Issue 15 (gender/sexuality overhaul)
Save/load world seed and game state
Farming, food, shelter resources
Death types (starvation, exposure etc)
Basic settings/config

## Known Issues

Detailed breakdown of currently open GitHub issues, captured here so it doesn't only live on GitHub. Last reviewed 16 July 2026.

### #15 — Gender/sexuality overhaul
Non-trivial, multi-stage. May split into a separate Milestone with multiple issues.
-Stage 1 (Gender off Name onto Person, separate BiologicalSex field) — mostly already done, Person already has Gender + Biosex as separate fields
-Stage 2 — Add non-binary to Gender enum, Gender/Biosex can misalign (trans representation), model-only
-Stage 3 — Add orientation to Person (hetero/homo/bi-pan/asexual/aromantic/aroace), add WillPair flag mirroring WillHaveChildren, a proportion of the population never pairs regardless of orientation
-Stage 4 (highest risk) — Pairing logic rewrite: reproduction eligibility via BiologicalSex, attraction/pairing eligibility via orientation, document the same-sex reproduction scope decision explicitly in code when this lands. Folds in #27's intersex requirement rather than solving it twice
-Stage 5 (optional) — Pronouns: She/Her, He/Him, They/Them to start, configurable pronoun sets so forks can add their own
-Architecture: sexuality/gender probability weights belong in AppSettings (existing IOptions pattern); assignment driven by the seeded DiceGenerator inside PersonGenerator, same shape as the existing CalcWillHaveChildren; config-driven probabilities let tests force e.g. HomosexualProbability: 1.0 for deterministic assertions
-Same-sex couples: non-reproducing pairing first; adoption (household tracking, child becomes eligible for adoption if no adult remains in their household) is a separate later system — people must never appear from nowhere
-Naming to resolve: the plan calls the field BiologicalSex, the code currently calls it Biosex

### #17 — Tuples should be dedicated result types
ProcessDeaths already returns Generation directly, not a tuple — already resolved.
GeneratePairs and GenerateChildren still return tuples — replace with named result types (e.g. PairingResult, ChildGenerationResult).
Open: exact naming, and whether to support tuple deconstruction for backward-compatible call sites.

### #19 — Simulation speed should be configurable
Heartbeat.cs hardcodes AddDays(1) per tick (existing TODO comment).
Decided: promote days-per-tick into AppSettings; death/birth rolls stay per simulated day — loop the roll once per day inside a multi-day tick rather than scaling the probability, to preserve the daily calibration in DeathModifier.
Still needed: SimulationManager's exact Day==01 && Month==01 new-year check must become a range check across the days a tick spans, or a tick that jumps over 1 January will silently skip New Year's and the generation-increment logic.

### #20 — O(n²) re-filtering in PairingEngine
singleMaleAdults is rebuilt from scratch on every female loop iteration instead of once (PairingEngine.cs:26).
Fix: materialize the single-male pool once, remove matched males directly instead of re-querying people.
Open tradeoff: swap-remove (true O(1) removal, O(n+m) total, but reorders the pool so existing world seeds would pair different people than before) vs ordered RemoveAt (same complexity class as today, far cheaper constant factor, preserves exact pairing outcomes for existing seeds). Given the project's reproducible-by-seed identity, ordered removal is the safer default but not yet confirmed.
Minor aside noticed nearby: singleMaleAdults.Count() uses the LINQ extension instead of the .Count property — not an actual perf issue (LINQ special-cases List<T>), just a style nit, optionally bundled into the same PR.

### #21 — Magic constant for fertility threshold
Git history: originally an int TimeFromLastChild >= 2; became DateTime .Year >= 3 after the int-to-DateTime refactor moved the reset baseline from 0 to Year 1 — same "2 years must pass" concept, different absolute number.
Duplicated in three places across two files: PersonGenerator.cs (twice, in the aliveFertilePairs filter) and AgeProcessor.cs (once, in GetTimeFromLastChild) — a private const scoped to PersonGenerator alone would miss AgeProcessor's copy.
Decided: promote to AppSettings so both classes read the same configured value instead of a shared constant class.

### #27 — Intersex inclusion in pairing/reproduction
Sex and Gender as separate properties on Person — already done (same fact as #15 Stage 1).
Not done: PairingEngine filters strictly on Biosex.Female/Biosex.Male; anyone with Biosex.Intersex is excluded from both pools entirely, can never be paired, and is therefore invisible to GenerateChildren too.
Open: fix as a narrow standalone change now, or fold entirely into #15 Stage 4's pairing rewrite so the eligibility model isn't redesigned twice. Not yet decided.

### Suggested tackling order
#17 (low-risk, do first so later changes build on named types) -> #21 (independent, anytime) -> #20 (decide swap-remove vs ordered-removal first) -> #15 + #27 together (#27 folds into Stage 4) -> #19 (can run in parallel, but cleaner once #15 Stage 4 settles the reproduction model).

## Pathway

A dependency-ordered path through the sections below — the era groupings alone don't capture what blocks what.

### Phase 1 — Stabilize the foundation
Work through Known Issues above
Issue 15 gender/sexuality overhaul — touches Person/PairingEngine directly, do before Genetics/Occupations/Dynasty add more on top
Save/load world seed + game state — do early, multiplies the value of every later phase and avoids restarting from scratch to test balance changes
Farming, food, shelter resources — mechanics only, no grid yet, same pattern as the existing Person/Pairing/Death systems
Death types (starvation, exposure) — only meaningful once resources exist
Basic settings/config — last, once it's clear what actually needs exposing

### Phase 2 — Era 2 core: grid & city builder
Grid system — spatial foundation Era 2 and Era 3's trains both depend on
City builder mechanics + 4 build variants x 4 colours (House/Farm/Garden/Wheat mill)
Proper main menu, settings UI
Cheese mode fits here or later — it's a UI reskin, needs real UI to exist first, not a day-one task despite being listed under Ongoing below

### Phase 3 — Genetics & representation
Expand InitialPeople.json beyond Welsh-inspired names, diverse founders — before genetics, not alongside; homogeneous founders make the trait system pointless from day one
Genetics system (hair/eye/skin, blending, mutation, dice-driven) — same probability pattern already used for biosex/pairing
Representation (skin tone as heritable trait) — this falls out of genetics as its flagship use case, not a separate system

### Phase 4 — Farming depth
Crop variety (Wheat/Rye/Barley/Potatoes)
Nutrition system — depends on crop variety existing, forcing diversification only means something once there's something to diversify

### Phase 5 — Social systems (can interleave with Era 2/3, not gated by Trains)
Dynasty & Family system (kinship graph, relatedness/inbreeding checks) — natural extension of the genetics data model
Occupations (roles, succession) — depends on Phase 2 buildings existing, a farmer role means nothing without a placeable farm
Murder — deliberately last here, depends on both Dynasty (feuds/vendettas across generations) and Occupations (motive tied to succession)
Lore documentation — lightweight, decoupled from code, worth writing early as a scope-creep guardrail

### Phase 6 — Era 3: Trains
Grid-based train building — reuses Phase 2's grid infrastructure directly
Passenger destination system — needs Occupations to be non-arbitrary
Pathfinding/demand modelling — most complex piece, correctly saved for last

### Continuous, not phase-gated
World gen refinement (seed-based, already started)
Blog documenting everything — write as each phase lands, not saved to the end
Scale constraint discipline (200+ population floor, no kingdoms) — a check to apply at every phase, not a deliverable itself

## Era 2 — MonoGame

Grid system
City builder mechanics
4 build variants + 4 colours per building variants
-House
-Farm
-Garden
-Wheat mill
Proper main menu, settings UI

### Genetics system

Heritable traits on Person: hair colour, eye colour, skin tone
Traits blend from both parents with variation
Driven by existing dice/probability pattern
Starting population must be diverse — this is foundational
Rare mutation chance for unusual traits (blue hair etc)
Connects to family tree system for trait tracing
Starting population considerations
Diverse by design — homogeneous founders = boring genetics forever
Worldbuilding decision: who survived the (unnamed) apocalypse
Name.json may need expanding beyond Welsh-inspired names to reflect this

### Representation

Skin tone as heritable genetic trait, not an assignment
Honest modelling produces natural diversity across generations
Cannot start everyone the same — document this as a design principle

### Farming
-Perfect oppatunity for crop variety
+Wheat
+Rye
+Barely
+Potatoes
+More crop variety = more fun
+Nutrition system - Forces players to diversify crops
	-This isn't Minecraft. Villagers can't just survive off wheat

## Era 3 — Trains

Grid based train building
Passenger destination system
Pathfinding/demand modelling

## Ongoing

World gen (seed based, already started)
Blog documenting everything
Cheese mode 🧀

### Dynasty & Family System

Family tree tracking — kinship graph on Person
Relatedness calculation to prevent inbreeding
"X is Y's cousin twice removed" level of detail
Lineage partially implicit already via name blending — formalise it

### Lore

Last survivors of an unnamed apocalypse
Apocalypse deliberately kept mystery — never explained
No scavenging, no fallout, no typical post-apocalyptic trappings
Fantasy/medieval aesthetic and tone
Small settlement scale — never loses sight of individuals
Document as design principles to guard against scope creep

### Occupations

Roles within the settlement (farmer, blacksmith etc)
Ties into food/shelter resource systems
Succession matters — who replaces the farmer when they die?
Connects to starvation/death systems

### Murder

Death type with an agent and a motive
Hooks into existing death system
Enables feuds, vendettas, family conflicts across generations
Connects to dynasty system naturally

### Scale constraint — For now

No kingdoms, no empires
Every person matters
Viable population target: 200+
Below 200 collapse is just maths. This has been researched by NASA scientists.