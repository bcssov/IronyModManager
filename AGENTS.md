# AGENTS.md

## Purpose

Irony Mod Manager is a mature cross-platform desktop application with more than six years of production history. The project is currently maintained primarily in life-support mode: stability, compatibility, dependency maintenance, targeted fixes, and focused improvements take priority over broad rewrites.

AI-assisted contributions are welcome. The use of AI does not lower or raise the engineering bar for a contribution. Contributors remain responsible for understanding, testing, reviewing, and supporting the changes they submit.

This file is the canonical repository guidance for coding agents. Tool-specific instruction files such as `CLAUDE.md`, `GEMINI.md`, or similar files must defer to this document rather than duplicate project policy. If guidance conflicts, `AGENTS.md` takes precedence.

## General Engineering Principles

- Preserve established behavior, especially in mature domain logic and compatibility-sensitive code.
- Existing structure is not automatically optimal. Refactoring is welcome when it produces clearer domain boundaries, better maintainability, or better testability without changing behavior unintentionally.
- Preserve behavior, not necessarily structure.
- Do not use class size, constructor size, method count, or complexity metrics as proxies for architectural quality.
- Irony follows SRP primarily along domain boundaries. A large class may still have a single domain responsibility.
- Conversely, if a large service contains genuinely separable responsibilities, extracting focused services and introducing a coordinator/facade is acceptable.
- Avoid repository-wide stylistic churn during dependency or framework maintenance.
- Local syntax modernization in touched code is welcome when it improves clarity and does not obscure the actual change.
- Do not treat unfamiliar or unusual code as obsolete merely because a newer or more fashionable pattern exists.
- When code appears unusual, inspect local Git history and related issues before changing it. Commit messages often contain `fixes #N` or `resolves #N` references that explain why compatibility code exists.
- Historical implementation choices may encode production lessons that are not obvious from the current source alone.

## Architecture

Irony uses a layered, modular architecture.

### Frontend

The frontend/UI layer consumes the service layer. It should not directly orchestrate lower-level infrastructure implementations.

### Services

The service layer is the application-level coordination layer. Services compose lower-level capabilities such as IO, storage, and game-data parsing into application behavior.

Services may contain substantial domain logic. In particular, Paradox-specific patching and conflict-resolution behavior has accumulated many real-world edge cases over the lifetime of the project.

### Lower-Level Components

Lower-level subsystems such as:

- IO
- storage/preferences
- game-file parsers
- similar infrastructure/domain components

are peer layers. They should remain independently focused and should not acquire lateral dependencies merely for convenience.

Application-level coordination belongs in the service layer.

Third-party implementation details should remain inside the layer that owns them. For example, archive libraries such as SharpCompress belong behind the IO abstraction rather than leaking upward into services or UI code.

### Contracts and Implementations

Irony separates contracts/common projects from implementation projects.

Implementation projects are intentionally not all directly referenced by the composition root. Do not add static project references merely because an implementation assembly is not visible at compile time.

The build and runtime composition model intentionally includes:

- independently built implementation assemblies
- build-time copying of implementation DLLs into the composition root/output
- runtime assembly discovery using naming conventions
- dependency discovery and resolution
- topological ordering
- DI/IoC registration and initialization

This is intentional architecture, not a missing-reference problem.

The loader also contains plugin/dependency resolution capabilities beyond the normal application path. Do not remove apparently unused plugin-resolution behavior as incidental cleanup without understanding its purpose and history.

## Dependency Injection

Irony uses dependency injection extensively.

Constructor size alone is not a defect. Some views/view-models or services may have many dependencies because of accumulated UI state or domain responsibilities.

Local cleanup is welcome where a clearer abstraction, facade, state object, or coordinator genuinely improves the design. Do not introduce indirection solely to reduce a parameter count.

## Paradox Domain Logic

Paradox game behavior is often irregular and has changed repeatedly over time.

Conflict resolution, mod patching, inline-script handling, parsing, filesystem behavior, load ordering, and related code may encode empirical compatibility knowledge collected from years of production use.

Refactoring these areas is allowed and may be desirable, but behavioral equivalence is the hard requirement.

When changing complex existing behavior:

- understand the current end-to-end semantics first
- preserve known edge cases
- use existing tests as regression protection
- add characterization/regression tests where the touched behavior is insufficiently protected
- prefer semantically meaningful decomposition over arbitrary splitting by file size

## Testing

Irony has a substantial business-logic test suite. Historically coverage was high, but maintenance has been lighter in recent years and current coverage must not be assumed.

The test suite primarily protects business logic. It is not a complete UI end-to-end test suite.

Green tests mean known contracts remain protected; they do not prove complete behavioral equivalence across every UI/platform path.

Do not pursue repository-wide coverage numbers as a goal in themselves. Add tests where they materially protect behavior being changed.

## Static Analysis

Release preparation includes PVS-Studio static analysis.

Existing analyzer configuration and suppressions are part of the established baseline. New warnings introduced by a change should be investigated rather than mechanically suppressed.

Static-analysis metrics are inputs to engineering judgment, not automatic architectural instructions.

## Avalonia

Avalonia major version 10 is an intentional compatibility boundary for Irony.

Do not upgrade Irony to a newer Avalonia major version as incidental dependency maintenance.

Irony contains Avalonia-version-specific behavior, custom controls, framework overrides, platform integration, and compatibility fixes that have accumulated over time.

Some framework workarounds and backports may:

- wrap Avalonia services
- subclass or override framework behavior
- use reflection into framework internals
- replace platform-specific behavior
- backport fixes from newer Avalonia versions

Such code is not automatically obsolete.

Before modifying Avalonia compatibility code:

1. inspect Git history
2. identify related issues where possible
3. understand the bug or compatibility requirement being addressed
4. preserve the existing behavior unless the current task explicitly changes it

Newer Avalonia source may be used as a reference for targeted fixes or backports. A newer upstream implementation is not automatically a migration target.

Major framework upgrades are product migrations, not routine package maintenance.

## Localization

Irony's localization architecture is stable and intentional.

- `en.json` is the canonical source of truth and default language.
- C# localization lookup members are generated from the canonical keys.
- Generated members provide compile-time protection when keys change.
- Runtime localization is applied through attributed virtual string properties and Castle proxy/interception behavior.
- UI elements bind to those properties.

Do not replace this architecture with ad-hoc string-key lookup or a generic framework-localization mechanism as incidental modernization.

Community translations may evolve independently, but canonical localization keys originate from `en.json`.

## Storage and Preferences

Preferences/storage are version-aware by design.

Versioned persistence files allow schema migration while preserving rollback compatibility with older Irony versions. Do not collapse versioned stores into a single unversioned settings file or alter migration semantics as incidental cleanup.

When a preference/storage model changes, preserve the established versioning and migration behavior.

## Dependency Upgrades

Dependency updates are welcome, including upgrades with breaking API changes.

For breaking dependency migrations:

- first identify the abstraction boundary that owns the dependency
- preserve that boundary where possible
- fix compile errors in the owning implementation rather than leaking new third-party APIs upward
- preserve old Irony behavior, not merely successful compilation
- run relevant tests
- add targeted regression coverage where needed

Do not combine unrelated framework migration, package upgrades, and broad refactors unless the changes are genuinely coupled.

## .NET Runtime

Irony may move to newer supported .NET runtime versions as maintenance requires.

A .NET runtime upgrade does not imply a corresponding Avalonia major-version upgrade.

Keep framework/runtime migration diffs focused and avoid opportunistic architectural rewrites unless they directly improve the touched area with a controlled blast radius.

## Build and Composition

Build scripts that copy implementation assemblies into the composition root are part of the application composition model.

Do not replace them with direct project references or redesign the composition pipeline as incidental cleanup.

When changing target frameworks, output paths, packaging, or build tooling, verify that:

- implementation assemblies are still copied to the expected location
- assembly discovery still sees the expected DLL set
- dependency resolution/topological ordering remains intact
- DI registration and initialization still occur correctly
- development and packaged layouts remain compatible with the runtime loader

## Versioning and Release Flow

Irony uses Nerdbank.GitVersioning.

Normal development occurs on `develop`, which produces alpha builds.

Release preparation uses a release branch. The established flow is:

1. prepare the release candidate using NBGV
2. produce one or more RCs as needed
3. once stable, prepare the final release state so the RC prerelease tag is removed
4. merge the release into `master`
5. merge `master` back into `develop`

Beta releases are not part of the normal flow.

Do not invent manual version numbers or replace the existing NBGV release process.

## AI-Assisted Contributions

AI-assisted pull requests are explicitly welcome.

The project does not require contributors to disclose which model or tool they used.

The same standards apply to all contributions:

- understand the change
- keep the PR focused
- explain intent
- preserve architectural boundaries
- test relevant behavior
- respond to review
- remain responsible for the submitted code

If an AI coding tool requires a repository-specific instruction file that does not yet exist, contributors should add the minimal adapter file required by that tool.

Tool-specific files must:

- direct the tool to read and follow `AGENTS.md`
- state that `AGENTS.md` is authoritative
- avoid duplicating general project policy
- contain only tool-specific bootstrap guidance that cannot reasonably live here

General project guidance belongs in `AGENTS.md`, not in tool-specific adapter files.

## Maintainer Context

Irony is a mature OSS application and has been in life-support maintenance for several years. Some cleanup and refactoring opportunities were intentionally deferred because maintainer time and interest shifted elsewhere.

Do not assume deferred cleanup is an architectural invariant.

At the same time, do not assume mature code should be rewritten simply because a cleaner modern implementation can be imagined.

Use engineering judgment:

- preserve learned behavior
- improve structure where the payoff is real
- keep blast radius understandable
- prefer domain clarity over cosmetic purity
