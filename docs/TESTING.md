# Testing and code coverage

## Testing policy

Irony was designed from the beginning for strong unit-test coverage of the business, domain, and deterministic application logic for which unit tests provide useful evidence. The normal unit-test surface includes:

- Services/business and application-domain behavior;
- Parser/domain behavior;
- Storage and IO logic;
- Models, Shared, and Localization where meaningful;
- limited deterministic frontend code, such as converters and DI/IoC resolution sanity.

Coverage does not mean every line in every first-party assembly. The following belong to other testing layers and are not missing ordinary unit coverage:

- UI interaction and UI state behavior, including views/viewmodels explicitly marked for functional testing;
- platform-specific behavior;
- DI/IoC implementation plumbing;
- framework/logging plumbing;
- XAML/AXAML markup;
- code explicitly excluded through the standard or Irony-specific coverage attributes.

Do not pull those surfaces into the unit-coverage denominator or create artificial UI/plumbing tests merely to increase a global percentage.

`Test.runsettings` is the authoritative coverage filter. It excludes:

- `IronyModManager.Platform`;
- `IronyModManager.DI` implementation/plumbing;
- test and third-party assemblies;
- `.xaml` and `.axaml` sources;
- code marked with `System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute`;
- code marked with `IronyModManager.Shared.ExcludeFromCoverageAttribute`.

The Irony-specific attribute has existed since 2020. Its `Reason` records the intended test boundary, such as functional/UI behavior, trivial generic carriers, or framework/logging plumbing. Preserve and heed that explanation rather than treating the exclusion as accidental missing coverage.

## Contributions and coverage

Coverage is descriptive, not a contribution KPI. AI-assisted contributors are encouraged to add focused characterization or regression tests when they change unit-testable behavior. Do not remove or bypass an exclusion without understanding its Reason and intended testing layer. If actual tooling contradicts this documented policy, report the discrepancy rather than silently redefining the denominator.

## Validation scope

Validation scope must match change scope. Relevant regression confidence matters more than maximizing the number of unrelated tests executed. A full regression gate required by an earlier high-blast-radius task does not automatically carry forward into a later narrow follow-up.

### Focused development loop

During implementation, run the directly affected tests first and iterate only on the affected test project or projects. Do not repeatedly rebuild or execute unrelated projects.

### Narrow and local changes

For a change confined to one feature, service, ViewModel, command, or semantic path:

- run focused regression tests that exercise the changed behavior;
- run the directly affected maintained test suite or suites;
- run the smallest meaningful build needed to prove the affected production code compiles and, where relevant, composes.

Do not run unrelated IO, Parser, Storage, UI, Services, or other suites merely to produce a repository-wide test count. Examples include a context-menu command correction, one membership or equality bug, localized ViewModel behavior, and a narrow service-method correction.

### Cross-layer and shared-contract changes

Broaden validation when a change affects shared interfaces or models, persistence contracts, DI/container composition, interception or proxy contracts, ReactiveUI coordination across layers, or code used materially by several maintained projects. Run every materially affected suite and use a full solution build when composition or the affected architecture requires it.

### Full canonical validation

Run all canonical maintained suites when scope justifies a repository-wide regression gate, including:

- explicitly high-blast-radius tasks;
- cross-cutting architecture changes;
- filesystem trust or safety architecture;
- broad persistence or domain changes;
- an explicit owner or master-chat request for a full regression gate;
- substantial work immediately before owner functional QA when broad qualification is warranted;
- RC or release qualification;
- evidence suggesting broader regression risk.

When a full canonical pass is justified, run the seven maintained test projects individually because solution-level CLI traversal can produce incomplete or misleading discovery/output:

- `IronyModManager.IO.Tests`;
- `IronyModManager.Localization.Tests`;
- `IronyModManager.Model.Tests`;
- `IronyModManager.Parser.Tests`;
- `IronyModManager.Services.Tests`;
- `IronyModManager.Storage.Tests`;
- `IronyModManager.Tests`.

Use Release, one MSBuild worker, and the repository's `net10.0` test target for the .NET 10-based 1.28 release. `FUNCTIONAL_TEST` cases are maintainer investigation probes that may access installed games and machine-specific paths; their normal skipped state is intentional.

### Build efficiency

Run heavy build, test, coverage, and publish operations sequentially. Avoid rebuilding the same graph unnecessarily. When a required build has already produced usable test assemblies, subsequent runs may reuse those outputs where the tooling supports it. Do not repeatedly attempt `--no-build` runs for assemblies that have not been produced. Preserve validation confidence while avoiding pointless resource consumption.

### Agent .NET process hygiene

Every agent-run .NET build or test sequence owns cleanup of processes it starts. This includes successful runs and failure, timeout, cancellation, interruption, and retry paths; arrange cleanup as a `finally` operation rather than success-only housekeeping. Process names alone do not establish ownership. Before starting a runner, record its root PID and start time and, while it is active, record descendant PIDs needed to identify its build/test tree. After completion, wait briefly for those recorded children to exit, then terminate only recorded, still-running descendants that are confirmed to belong to that invocation. Do not terminate a process merely because it is named `dotnet`, `MSBuild`, `VBCSCompiler`, `testhost`, or `vstest.console`.

The owner workstation has 64 GB RAM. Treat substantial memory growth, workstation responsiveness degradation, or any retained worker/test process from an agent invocation as a validation failure even if the build and tests report success. Report that failure; do not begin another heavy build/test pass merely to obtain totals while the machine is already under substantial memory pressure.

Never use `taskkill /F /IM dotnet.exe`, `kill-dotnet.bat`, or any equivalent process-name-wide termination from an agent. Such commands can stop the owner's Visual Studio session, another terminal build, another agent, or a legitimate .NET application. A process that predates the agent command is external unless its ownership is positively established.

Use the current runner's supported non-persistent mode where practical:

- The installed .NET 10 SDK supports `--disable-build-servers` for `dotnet build` and `dotnet test`; include it when an agent-run command performs a build. Keep `--no-build` when valid so focused tests can reuse a verified prior build rather than rebuilding solely for hygiene.
- The installed Visual Studio MSBuild 18.10 supports `-nr:false`; use it on every direct agent-run `MSBuild.exe` build/test preparation command unless a concrete toolchain reason prevents it and is reported. VSTest has no equivalent reusable-node switch in this workflow; its `vstest.console` and `testhost` processes must instead be tracked as children of the recorded invocation and allowed to exit normally.
- The installed SDK supports `dotnet build-server shutdown`, which gracefully stops build servers started from `dotnet`. It is appropriate only when the agent has an exclusive, positively identified CLI build-server context. Do not run it as routine desktop cleanup, because its documented default stops all dotnet-started build servers and may affect other work.

Keep heavy validation sequential as required above. This makes PID-tree attribution reliable as well as reducing resource contention. Hygiene does not justify redundant builds: select focused/affected validation first and reuse valid outputs where supported.

### Reporting

For narrow tasks, report the focused tests executed, affected maintained-suite totals, and the relevant build result. A repository-wide test total is required only when a repository-wide canonical pass was justified and actually executed. Do not present the absence of a global total as incomplete validation for a narrow task.

## Visual Studio coverage

Visual Studio's Analyze Code Coverage feature remains the historical interactive workflow. It requires an edition that provides code coverage.

1. Open `IronyModManager.sln` and perform the repository's normal full-solution Rebuild.
2. Select the root `Test.runsettings` through **Test > Configure Run Settings > Select Solution Wide runsettings File**. Do not rely on solution inclusion alone; verify that the file is selected.
3. Confirm Test Explorer has discovered the seven projects above.
4. Choose **Test > Analyze Code Coverage > All Tests**, or the equivalent Analyze Code Coverage action in Test Explorer.
5. Confirm the test result and inspect the Code Coverage Results window.
6. Verify that DI, Platform, test/third-party modules, `.xaml`/`.axaml`, and representative attributed types are absent, while Services, Parser, and Storage business code remains present.

On 2026-09-10, the owner executed this workflow in Visual Studio with the corrected settings and obtained 61.80% line coverage and 64.17% block coverage. This confirmed that Visual Studio instrumentation no longer produced empty results after the test-assembly exclusion was narrowed to assembly filenames rather than matching parent test-project directories. Visual Studio and CLI percentages need not be byte-for-byte identical because their collectors and symbol mapping can calculate coverage differently.

## Command-line coverage

There are three distinct command-line cases:

- `dotnet test` executes tests only.
- `dotnet test --settings Test.runsettings --collect "Code Coverage"` asks VSTest to use dynamic profiler collection. With Microsoft.NET.Test.Sdk 18.10.0, it was revalidated successfully: the test run completes and emits a usable `.coverage` artifact without a profiler-initialization failure.
- Microsoft `dotnet-coverage` with `--include-files` statically instruments the selected generated assemblies. It remains the canonical automated CLI workflow because it is reproducible across all seven test projects.

Install the Microsoft tool; version 18.10.0 established the baseline below:

```powershell
dotnet tool install --global dotnet-coverage --version 18.10.0
```

From the repository root, build and collect each project separately:

```powershell
$tests = @(
    'IronyModManager.IO.Tests',
    'IronyModManager.Localization.Tests',
    'IronyModManager.Model.Tests',
    'IronyModManager.Parser.Tests',
    'IronyModManager.Services.Tests',
    'IronyModManager.Storage.Tests',
    'IronyModManager.Tests'
)

New-Item -ItemType Directory -Path 'TestResults\Coverage' -Force | Out-Null

foreach ($name in $tests) {
    $project = "src\$name\$name.csproj"
    $bin = "src\$name\bin\Release\net10.0"

    dotnet build $project -c Release -m:1 --disable-build-servers
    if ($LASTEXITCODE -ne 0) { throw "Build failed: $name" }

    dotnet-coverage collect `
        --settings Test.runsettings `
        --include-files "$bin\IronyModManager*.dll" `
        --output "TestResults\Coverage\$name.xml" `
        --output-format xml `
        --nologo `
        dotnet test $project -c Release -m:1 --no-restore --no-build `
        --logger 'console;verbosity=minimal'
    if ($LASTEXITCODE -ne 0) { throw "Coverage failed: $name" }
}

dotnet-coverage merge TestResults\Coverage\*.xml `
    --output TestResults\Coverage\merged.xml `
    --output-format xml `
    --nologo
```

Static collection temporarily instruments generated assemblies. The baseline run verified that `dotnet-coverage` restored every instrumented DLL byte-for-byte. If collection is interrupted, use the repository clean workflow and rebuild before further testing or packaging; do not trust a possibly instrumented `bin` tree.

## Dated baseline

On 2026-09-10, the seven project runs produced 894 passed, 14 intentionally skipped, 0 failed, and 908 total tests.

- Visual Studio Analyze Code Coverage: **61.80% lines** and **64.17% blocks**.
- Microsoft `dotnet-coverage` static CLI workflow: 6,542 covered, 18 partially covered, and 3,224 missed lines, or **66.86% line coverage**.

Both results measure the configured applicable unit-test surface. Their percentages are separate collector baselines rather than values expected to match exactly.

| Included module | Line coverage | Covered / partial / missed |
|---|---:|---:|
| IronyModManager.Localization | 100.00% | 110 / 0 / 0 |
| IronyModManager.Shared | 25.03% | 191 / 0 / 572 |
| IronyModManager.IO.Common | 36.26% | 33 / 0 / 58 |
| IronyModManager.Models.Common | 100.00% | 4 / 0 / 0 |
| IronyModManager.Models | 38.27% | 31 / 0 / 50 |
| IronyModManager.Storage | 94.48% | 137 / 0 / 8 |
| IronyModManager.IO | 52.19% | 334 / 0 / 306 |
| IronyModManager.Parser | 86.21% | 1,826 / 0 / 292 |
| IronyModManager.Parser.Common | 93.15% | 340 / 0 / 25 |
| IronyModManager.Services.Common | 96.30% | 26 / 0 / 1 |
| IronyModManager.Services | 64.03% | 3,356 / 0 / 1,885 |
| IronyModManager frontend selected sources | 77.39% | 154 / 18 / 27 |
| **Aggregate** | **66.86%** | **6,542 / 18 / 3,224** |

The merged report contained exactly those 12 first-party modules and no third-party or test module. It excluded `ConflictSolverResetConflictsControlViewModel`, `CommandResult<T>`, `AvaloniaLogger`, all markup, DI, and Platform. It retained representative business code including `ModPatchCollectionService`, `GameIndexService`, `ParserManager`, `Database`, and `Storage`.

Reliable aggregate branch coverage is not claimed: this static XML reported no block data for 11 of the 12 modules.

Historical coverage data from the early DesktopFramework lineage from which Irony evolved, during the .NET Core 3.1 and Avalonia 0.9 era, confirms approximately 90% or higher unit-test coverage across the intended testable surface. It is evidence of Irony's engineering philosophy, not a claim that the current Irony assembly set was measured at that exact number or a numeric target for current work. Over roughly six years, Irony's production and Paradox-domain surface grew substantially while maintaining an aggregate percentage stopped being an active development goal; the reconstructed 1.28 CLI baseline is 66.86% over the equivalent applicable unit-test surface. Collector behavior has also changed. An earlier approximately 34.4% ad-hoc result is not canonical: stale collection admitted third-party, UI, and markup code and did not reproduce the intended denominator.
