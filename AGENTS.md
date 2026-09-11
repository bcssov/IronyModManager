# AGENTS.md

## Purpose and authority

Irony Mod Manager is a mature cross-platform desktop application with more than six years of production history. It is maintained primarily in life-support mode: stability, compatibility, dependency maintenance, targeted fixes, and focused improvements take priority over broad rewrites.

AI-assisted contributions are welcome. AI use does not lower or raise the engineering bar. Contributors remain responsible for understanding, testing, reviewing, explaining, and supporting the changes they submit.

This file is the canonical repository guidance for coding agents and is also useful to human contributors. Tool-specific instruction files such as `CLAUDE.md`, `GEMINI.md`, or similar files must defer to this document rather than duplicate project policy. If guidance conflicts, `AGENTS.md` takes precedence.

The current Irony repository and its current behavior are authoritative. Historical repositories, architectural lineage, external forks, old wiki material, upstream/donor framework source, and comments that contradict current tested behavior are context only. They may explain intent, but they do not define current behavior.

## Core engineering principles

```text
Preserve behavior, not necessarily structure.
```

- Preserve established behavior, especially in mature domain logic, persistence, security boundaries, and compatibility-sensitive code.
- Existing structure is not automatically optimal. Refactoring is welcome when it produces clearer domain boundaries, lifecycle ownership, maintainability, or testability without unintended behavior changes.
- Evaluate single responsibility primarily by domain responsibility, not source-line count, constructor size, method count, or complexity metrics.
- A large service may legitimately represent a large domain. It may also contain real semantic seams worth extracting.
- Avoid repository-wide stylistic churn during framework or dependency maintenance.
- Local syntax modernization in touched code is welcome when it improves clarity and does not obscure the actual change.
- Do not classify unfamiliar code as obsolete merely because a newer pattern exists. Inspect current callers, tests, Git history, and related issues first.
- Commit messages commonly reference issues with forms such as `fixes #N` and `resolves #N`; those issues are part of the design evidence for compatibility code.

## Architecture and dependency direction

Irony uses a layered, modular architecture. The normal application direction is:

```text
UI / frontend
     |
     v
Services / application-domain coordination
     |
     +--> IO contracts / implementation
     +--> Parser contracts / implementation
     +--> Storage contracts / implementation
     +--> other lower-level capabilities
```

### Frontend and services

The frontend primarily consumes the service layer. It should not directly coordinate lower-level implementations.

Services form the application coordination layer. They combine lower-level capabilities into user-facing workflows and may also own substantial application/domain behavior.

### Lower-level capabilities

IO, storage/preferences, game-file parsing, platform integration, and similar components are lower-level capabilities. Avoid lateral dependencies between them merely for convenience; application-level orchestration belongs in Services.

Dependencies on stable lower-level contracts can be legitimate. Storage's dependency on `IO.Common` and `DiskOperations` is an accepted example because `DiskOperations` owns filesystem operations. Do not classify that dependency as a layering defect without a concrete, semantically better boundary.

Third-party implementation details must remain behind the abstraction that owns them. Archive-library types belong inside IO, persistence-library details inside Storage, parser-engine details inside Parser, and framework-specific behavior inside the relevant UI/Platform boundary.

### Contracts, implementations, and composition

Contract/common projects are intentionally separated from runtime implementations. The executable/composition root intentionally does not statically reference every implementation project.

The application composition path is intentional:

1. implementation projects are built independently;
2. their output assemblies are copied into the executable layout;
3. runtime discovery finds candidate Irony assemblies by convention;
4. Irony's identity policy validates dynamically discovered module assemblies;
5. module dependencies are discovered and ordered;
6. DI packages and other runtime registrations are discovered;
7. modules are composed and initialized.

Missing direct project references are therefore not evidence of broken architecture. Do not add references merely to make runtime implementations visible to the executable at compile time.

Build the full solution when validating composition. Building only the application project can leave implementation outputs missing or stale before the dependency-copy step. When changing target frameworks, output paths, packaging, or build tooling, verify the copied DLL set, runtime discovery, dependency ordering, DI registration, initialization, and both development and packaged layouts.

The loader also supports dependency resolution through plugins and in-memory paths beyond the ordinary packaged application flow. Code is not dead merely because the common deployment path does not exercise every loader capability.

### Repository-owned build and maintenance scripts

Before writing an ad-hoc command for an operation below, use the repository-owned script or establish why it is unsuitable. These batch files assume they are started with `cmd` as the working directory; several begin by moving to the repository root, and `copy-dependencies.bat` receives its paths from MSBuild.

- `build-tools.bat` builds the localization resource generator in Release and copies its executable, assemblies, and runtime configuration into `Tools\LocalizationResourceGenerator`. Run it before a build that needs a missing or changed generator. `run-tools.bat` runs that prepared generator and therefore follows `build-tools.bat`; generation can update generated localization sources, so review its diff rather than invoking it as general cleanup.
- `update-versioning.bat` updates the globally installed Nerdbank.GitVersioning CLI. Use it only when the maintainer workflow requires updating `nbgv`; it changes a user-level tool installation and is not a normal solution-build prerequisite.
- `clean-solution-partial.bat` removes project `bin` directories, localization-generator `bin`, and `TestResults`, while retaining `obj`. Use it for ordinary generated-output cleanup. `clean-solution-full.bat` removes both `bin` and `obj` plus `TestResults`; use it before dependency-removal, package-layout, target-framework, or clean-machine-style validation. The scripts may report missing directories and return a nonzero status when targets are already absent, so verify the generated-directory state when that is the only reported condition.
- `copy-dependencies.bat` is the composition action invoked by the main executable project's explicit composition target. Build-only project references order the runtime implementation builds without adding their outputs as compile/runtime references; the script then copies those exact project outputs and required reference assets. It reconciles primary implementation artifacts through its generated manifest. Transitive third-party files remain additive, so clean first when validating removal of one of those dependencies or packaged contents.
- `kill-dotnet.bat` force-terminates every `dotnet.exe` process, while `kill-process.bat` force-terminates running `ironymodmanager.exe` processes. Use them only when the corresponding process is blocking an intentional clean/build operation, after considering unrelated .NET work on the machine. Do not substitute broad process-kill commands or use either script as a routine build step.

A normal full solution Build populates the independently built outputs and then composes the executable layout. Rebuild forces recompilation but must produce the same composition semantics; it is not required to obtain a valid layout. Use the full solution for CLI composition validation, and do not manually invoke `copy-dependencies.bat` with invented paths or use it as a substitute for building the full solution.

## Dependency injection and refactoring

Irony's architecture deliberately adopted Simple Injector's explicit dependency style after extensive experience with Ninject. Explicit constructor dependencies remain valuable, but the architecture has never prohibited deferred creation or interception. It includes concepts such as:

- `Func<T>`;
- `Lazy<T>`;
- parameterized factories;
- proxy/interception-based behavior.

Use these principles when reviewing or refactoring dependency-heavy code:

- constructor size is evidence, not a verdict;
- keep important dependencies and ownership visible;
- factories are appropriate when they own meaningful object creation or parameterized construction;
- facades and coordinators are appropriate when they express a real domain workflow or orchestration boundary;
- aggregate state objects and lifecycle owners are appropriate when they own coherent state or lifetime;
- parameter bags or facades that merely conceal a long constructor are not useful architecture;
- preserve behavior and keep the blast radius controlled.

Do not try to reconstruct an unavailable successor architecture. Apply the durable semantic lessons above to the current code and task.

## Paradox domain behavior

Paradox game behavior is irregular and has changed repeatedly. Conflict solving, patching, parsing, load order, inline scripts, parameterized constructs, game indexing, filesystem behavior, localization conflict rules, and related logic may encode years of empirical production knowledge.

Three AI-assisted attempts to replace Irony have historically stalled around this class of behavior. The product's value is not merely its visible structure; it includes accumulated compatibility knowledge.

Refactoring these areas is allowed and can be valuable. Behavioral equivalence is the hard requirement.

Before changing complex domain behavior:

1. reconstruct the current end-to-end semantics;
2. inspect relevant tests, history, and issues;
3. preserve known edge cases and fallback behavior;
4. add focused characterization/regression coverage when the changed behavior benefits from explicit protection;
5. extract only semantically meaningful policies, lifecycle owners, builders, factories, or coordinators.

When parser behavior or any definition-cache schema, serialization, persistence, indexing, or normalization behavior changes, review the affected `GameRegistration` game-index cache version(s). Treat each cache version as a semantic/cache-schema epoch: increment it when existing cached definitions may no longer represent the current behavior or persistence contract, but not for a purely internal refactor that provably cannot affect either. If only some games are affected, bump only those games so their disposable caches are regenerated.

Do not split a class merely to reduce its size.

## Avalonia and platform compatibility

Avalonia 0.10.x (Irony's Avalonia 10 compatibility boundary) is intentionally pinned. Do not propose or perform a routine Avalonia major upgrade as incidental dependency maintenance. A major framework upgrade is a product migration and is outside normal 1.28 maintenance.

If `CompileAvaloniaXamlTask` reports an `original.pdb` sharing violation, see [Tools/Avalonia.Build.Tasks.Fix/README.md](Tools/Avalonia.Build.Tasks.Fix/README.md) for the optional Avalonia 0.10.22 workaround. It is not a normal build prerequisite.

Irony contains framework-version-specific behavior, including:

- custom controls and templates;
- framework overrides and wrappers;
- reflection-based compatibility shims;
- selected fixes and backports from later Avalonia versions;
- X11 and Wayland integration;
- platform-specific input, window, dialog, clipboard, rendering, and lifecycle workarounds.

Before removing or modernizing unusual framework code:

1. inspect its current callers;
2. inspect Git history and the originating issue or behavior where discoverable;
3. understand the platform/framework failure it prevents;
4. preserve the behavior unless the task explicitly changes it;
5. validate on the affected platform path where practical.

Newer Avalonia source may be used as a donor/reference for a targeted fix or backport. It is not automatically a migration target. Irony's own localization system is separate from Avalonia-specific work needed to localize or correct framework/control behavior.

## Localization

Irony's localization architecture is stable and intentional:

1. `en.json` is the canonical/default resource and source of localization keys.
2. The existing localization generator/build-tools flow produces C# lookup members from those keys.
3. Generated C# members form a compile-time contract for lookup sites.
4. Attributed virtual properties expose localized values to the UI.
5. Castle proxy/interception behavior resolves values and performs runtime locale refresh.

Do not hand-edit generated localization files. Key additions, removals, and renames originate in `en.json` and flow through the existing generator. Do not replace this architecture with ad-hoc string-key lookup or generic framework localization during unrelated work.

## Testing strategy

Irony has a substantial xUnit business/domain suite. Coverage refers to the applicable unit-test surface, not every line in the frontend or every first-party assembly. Services, Parser, Storage, IO, Models/Shared, Localization, and limited deterministic frontend code are the normal unit-test surface. UI interaction/state, explicitly functional-tested view/viewmodel behavior, platform work, DI/IoC implementation plumbing, framework/logging plumbing, and XAML/AXAML belong to other testing layers. Do not pull them into the denominator or create artificial tests merely to increase a global percentage.

Honor both `System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute` and `IronyModManager.Shared.ExcludeFromCoverageAttribute`. The Irony-specific attribute's `Reason` documents why code belongs outside ordinary unit coverage and is part of the test-strategy guidance. Detailed policy, Visual Studio steps, the canonical CLI coverage workflow, and the dated baseline are in [`docs/TESTING.md`](docs/TESTING.md).

Add focused characterization/regression tests for changed unit-testable behavior, not to chase an aggregate. Do not remove or bypass an exclusion without understanding its Reason and intended testing layer. Report a tooling/policy discrepancy rather than silently redefining the denominator.

Visual Studio's xUnit integration is the historical primary runner. If solution-level CLI `dotnet test` exits successfully without useful discovery or test-result output, do not accept that as evidence that tests ran. Invoke the seven test projects explicitly:

- `IronyModManager.IO.Tests`;
- `IronyModManager.Localization.Tests`;
- `IronyModManager.Model.Tests`;
- `IronyModManager.Parser.Tests`;
- `IronyModManager.Services.Tests`;
- `IronyModManager.Storage.Tests`;
- `IronyModManager.Tests`.

At the 2026-09-10 coverage baseline, those project-level runs discovered 908 tests: 894 passed and 14 were intentionally skipped. This is a dated baseline, not a permanent test-count invariant.

`FUNCTIONAL_TEST` tests are intentional maintainer investigation probes. They may perform real IO, scan installed games, contain machine-specific paths, and emit evidence about new Paradox content. They are not normal portable unit tests. Do not treat their default skipped state as broken coverage, remove them as dead tests, or force them into ordinary contributor validation.

Green tests protect known contracts; they do not establish complete behavioral equivalence for every UI, platform, packaging, or game-data path.

## Static analysis

PVS-Studio is a manual, maintainer-local release-time check. There is intentionally no repository-owned PVS configuration or baseline for contributors to reproduce.

Contributors and coding agents are not required to install PVS-Studio or any commercial/external analyzer, and PVS is not a contribution gate. Do not add repository suppressions or configuration in an attempt to reproduce a private maintainer workflow unless explicitly requested.

Compiler/analyzer warnings encountered in normal project tooling should still be investigated. Static-analysis metrics are inputs to engineering judgment, not architectural commandments.

## Storage and preferences

Preferences and persisted application state are version-aware by design. Persistence files are versioned by the running application's major/minor version so a newer release can migrate state while retaining the older version's compatible state for rollback.

The migration invariant is:

```text
Migration fallback must never select a persistence schema newer than the
running application's major/minor version.
```

When changing storage models or migration behavior, preserve:

- writes to the current version's store;
- migration from eligible older stores;
- source-file retention;
- crash-recovery behavior;
- rollback/copy-on-write behavior.

Do not collapse versioned persistence into a single unversioned settings file as incidental cleanup.

## Dependency maintenance

Dependency upgrades are ownership-driven. For any upgrade, especially one with breaking APIs:

1. identify the abstraction that owns the dependency;
2. keep third-party APIs behind that abstraction;
3. preserve observable Irony behavior rather than merely achieving compilation;
4. inspect deliberate pins, source comments, history, and related issues before changing them;
5. run the relevant business/domain tests and add focused regression fixtures where needed;
6. keep unrelated framework migrations and refactors separate unless genuinely coupled.

Archive and image input-processing changes deserve corrupt/adversarial fixtures and path-boundary validation. Archive extraction changes must preserve Irony's canonicalization/root-containment boundary: every filesystem extraction path must sanitize an entry before a third-party write API receives the destination.

Do not assume an old dependency must be upgraded if its historical responsibility can instead be removed. Prefer deleting obsolete compatibility providers/stacks when evidence from real supported files proves they are no longer needed; if a fallback remains necessary, retain the smallest one and document the behavior it owns.

A .NET runtime upgrade does not imply an Avalonia upgrade. Keep runtime migration focused and verify hard-coded target-framework output paths, copied implementation assemblies, runtime composition, native dependencies, and packaged layouts.

## Runtime assembly identity and trust

Irony dynamically discovers runtime module assemblies. The plugin model was deliberately designed to be powerful: plugins can participate in plugin assembly discovery and DI registration, replace or add application components, declare dependency order, provide localization, run post-startup behavior, and consume exposed application integration points. That power is why identity validation was designed alongside dynamic composition rather than added as incidental signing machinery.

Historically, authorization to receive plugin-signing capability followed maintainer vetting and approval. The runtime identity gate then enforces possession of the accepted signing identity; it does not independently reproduce or prove that the maintainer approval process occurred. Do not provision, broaden, or alter that trust without explicit maintainer direction.

Strong-name/public-key metadata participates in Irony's own application-level identity policy for candidate module assemblies. This is not a claim that CLR strong names are a general security sandbox. It is an Irony composition invariant: dynamically discovered candidate module assemblies must match the trusted identity expected for their discovery path. An unexpected identity is a security failure and stops execution before the assembly can proceed through module metadata discovery, dependency ordering, DI/runtime registration, and initialization.

The current finder must load or obtain an `Assembly` object before it can read and validate that identity. Do not inaccurately describe this policy as validation before CLR loading or before an assembly exists in memory. The security boundary is the application composition/execution path after discovery/loading and before the candidate is accepted for composition.

Do not remove, bypass, weaken, or casually refactor:

- public-key/strong-name validation for dynamically discovered module assemblies;
- the trusted embedded identity material used by that validation;
- signing configuration for official Irony assemblies;
- identity checks associated with plugins or dynamic composition.

Generic guidance that strong names are not a modern sandbox is not sufficient reason to remove Irony's application-level identity gate. Validate the complete discovery-to-composition call chain before changing it.

The public plugin ecosystem did not grow enough to justify a more elaborate trust service, so public Irony retained its local identity mechanism. The public repository's current behavior is authoritative. Do not infer, reconstruct, document in detail, or imitate unavailable proprietary validation systems.

## Update authenticity and release secrets

NetSparkle release/update artifacts use Ed25519 signatures for update authenticity. The private update-signing key is maintainer-only release security material, remains outside the public repository, and must stay uncommitted.

Contributors and coding agents must never:

- commit private signing keys or other release secrets;
- request that private keys be added to the repository;
- generate replacement official keys without explicit maintainer instruction;
- relocate private release keys into tracked source;
- weaken update signing because contributor builds cannot reproduce official signatures.

Contributor builds are not equivalent to official signed Irony release artifacts.

Runtime assembly identity signing and NetSparkle Ed25519 update signing are separate mechanisms with separate purposes. Do not conflate their keys, validation paths, threat models, or maintenance decisions. Both cross maintainer-authorized trust boundaries, and sensitive signing material must remain outside the public repository.

## Public builds and official releases

Reproducible contributor build/test guidance is valuable, but public CI/CD is intentionally not a 1.28 requirement. Do not treat its absence as merely missing workflow YAML.

Official publishing depends on maintainer-only security and infrastructure, including release signing, runtime assembly identity material, private/custom package availability, and local packaging/deployment state. Official publishing and signing remain maintainer responsibilities. Never commit secrets or redesign release infrastructure during ordinary contribution work.

A future public build/test system may be considered separately. It would validate contributor builds; it would not be equivalent to the official signed release pipeline or cross the same trust boundary.

## Versioning and release flow

Irony uses Nerdbank.GitVersioning. Normal development occurs on `develop`, producing alpha builds. Do not invent manual version numbers or replace the established NBGV flow.

The release flow is:

1. create/use the release branch;
2. prepare the release candidate with NBGV;
3. produce one or more RCs as needed;
4. when stable, prepare the final state so the RC prerelease marker is removed;
5. merge the release into `master`;
6. merge `master` back into `develop`.

Beta releases are not part of the normal flow.

## AI-assisted contributions

AI-assisted pull requests are explicitly welcome. AI use is not itself a reason to reject a contribution, and contributors do not have to disclose the model/tool used or provide prompts or chat logs.

The same standards apply to human-only and AI-assisted work. Contributors remain responsible for:

- understanding the change and its intent;
- keeping the PR focused;
- preserving architecture, security, and compatibility boundaries;
- explaining important decisions;
- testing relevant behavior;
- reviewing generated changes rather than submitting them blindly;
- responding to review and supporting the submitted code.

AI assistance neither lowers the engineering bar nor disqualifies a contribution.

If a tool requires a repository-specific instruction file that does not yet exist, a contribution may add the minimal adapter required by that tool. Such files must:

- direct the tool to read and follow `AGENTS.md`;
- state that `AGENTS.md` is authoritative;
- contain only tool-specific bootstrap guidance that cannot live here;
- avoid duplicating general project policy.

General project guidance belongs in `AGENTS.md`, not competing copies.

## Maintainer context

Irony has been in life-support maintenance for several years. Some cleanup was deferred because maintainer time moved elsewhere; deferred cleanup is not an architectural invariant.

Use engineering judgment:

- preserve learned behavior;
- improve structure where the semantic payoff is real;
- keep blast radius understandable;
- prefer domain clarity over cosmetic purity;
- keep official release/security responsibilities separate from contributor validation.
