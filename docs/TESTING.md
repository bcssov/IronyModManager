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

## Normal test execution

Ordinary `dotnet test` runs tests but does not inherently collect coverage. Run the seven normal test projects individually because solution-level CLI traversal can produce incomplete or misleading discovery/output:

- `IronyModManager.IO.Tests`;
- `IronyModManager.Localization.Tests`;
- `IronyModManager.Model.Tests`;
- `IronyModManager.Parser.Tests`;
- `IronyModManager.Services.Tests`;
- `IronyModManager.Storage.Tests`;
- `IronyModManager.Tests`.

Use Release, one MSBuild worker, and the repository's `net8.0` test target while 1.28 still targets .NET 8. `FUNCTIONAL_TEST` cases are maintainer investigation probes that may access installed games and machine-specific paths; their normal skipped state is intentional.

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
- `dotnet test --settings Test.runsettings --collect "Code Coverage"` asks VSTest to use dynamic profiler collection. In the current maintainer environment this runs the tests but produces no usable coverage and reports `Profiler was not initialized`.
- Microsoft `dotnet-coverage` with `--include-files` statically instruments the selected generated assemblies. This is the canonical automated CLI workflow until dynamic VSTest collection is independently shown to emit a real coverage artifact.

The bounded VSTest diagnostic showed the collector was loaded and supplied the profiler environment variables, but the profiler did not initialize. It also reported a collector/SDK extension dependency mismatch involving `Microsoft.Bcl.AsyncInterfaces`. Removing the obsolete VS-era collector binding did not repair collection, so this is treated as a current tooling limitation rather than a test or filtering failure.

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
    $bin = "src\$name\bin\Release\net8.0"

    dotnet build $project -c Release -m:1
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
