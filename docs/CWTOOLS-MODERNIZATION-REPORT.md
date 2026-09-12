# CWTools modernization review report

Date: 2026-09-12

CWTools is finalized and pushed as `bcssov/cwtools:irony` commit `0b550b0d6f56da60411b34ffc4df3ca27a0c3243`.

## Irony usage

Production use is confined to `IronyModManager.Parser/CodeParser.cs`. It calls `CWTools.CSharp.Parsers.ParseScriptFile(file, code)`, reads the returned FParsec result's `IsSuccess`, and, on failure, calls `GetError()` and consumes `Column`, `Line`, and `ErrorMessage`. `IronyModManager.Parser.Common` used no CWTools namespace, type, or method; its old package reference supplied only inherited compile assets and has been removed.

## Historical fork delta

The old `irony` tip was `1acc09fca52d9b9fc756a5ec2a7c20c354cf0769`; its common baseline with original upstream is `fdcfb9489559c3c25c3e38de760c1cd9ce3d993d`. There is no surviving source-tree delta between those points. Its only unique non-merge commit, `af76a944`, adjusted the historical Paket/build setup; later merges made that tree equal to its upstream baseline. `ParseScriptFile` was already common-lineage code.

Consequently, no historical Irony parser behavior had to be retained. The fork itself remains useful as the reviewed, reproducible source for Irony's binary build.

## Source comparison and classification

Selected original-upstream baseline: `cwtools/cwtools` `b377453dee803f9258be92cfc49896d09039702d` (2025-12-08).

Compared maintained-fork tip: `Aa728848/cwtools` `cec56ee167bf8324a59e59ad39acd24b175c3de4` (2026-09-04), 252 commits beyond that baseline.

| Classification | Decision |
| --- | --- |
| Required | Target `net10.0`; align source/build to `FSharp.Core` package 10.1.400; allow `?` inside an unquoted identifier so Stellaris `starbase?= {}` and `starbase? = {}` parse as key `starbase?` plus ordinary `=`. |
| Beneficial and low-risk | Original upstream's accumulated parser fixes/performance work and its C# wrapper; adapt only the single `idCharArray` addition from maintained commit `21e48dfd242d31caa77c314cb5d59de5cd7db28b`. |
| Irrelevant | Maintained-fork game/rules/editor/CLI additions and broad new game support outside Irony's validity-only path. |
| High-risk / architectural | Maintained-fork resource management, engine scaffolding, language-service/rules refactors, shader work, and wider parsing architecture. These were not imported. |

The maintained fork's .NET 10 commit `9ba278621b3abafbdc58199455aca3b112f5210c` informed the mechanical TFM update, but the final alignment was adapted: explicit `System.Text.Encoding.CodePages` references were removed because .NET 10 provides it and reports them as unnecessary.

Selected lineage is therefore **modern original upstream plus one narrowly proven maintained-fork parser fix**, not the maintained fork as a baseline.

## Build and dependency result

- Branch: `bcssov/cwtools:irony`, final commit `0b550b0d6f56da60411b34ffc4df3ca27a0c3243`.
- Source identity: original baseline `b377453d`, maintained fix adapted from `21e48dfd`, plus the reviewed .NET 10/FSharp alignment above.
- Configuration/TFM: Release, `net10.0` (`Shared.dll` remains `netstandard2.0`).
- Assembly versions: `CWTools`, `CSharpHelpers`, and `Shared` are all 1.0.0.0.
- FParsec: 1.1.1 / assembly 1.1.1.0. The old private package used 1.0.4-RC3 / assembly 1.0.4.0.
- FSharp.Core: package 10.1.400; resolved runtime assembly 10.1.0.0. Irony's existing exact pin is retained.

Modern `CWTools.dll` has runtime assembly references to both `CSharpHelpers.dll` and `Shared.dll`; `CSharpHelpers.dll` also references `Shared.dll`. Its helpers cover extensions, field-validator acceleration, and file-manager helpers. All three binaries are therefore legitimate direct runtime dependencies; none was copied into Irony source.

Imported SHA-256 values:

| Binary | SHA-256 |
| --- | --- |
| `CWTools.dll` | `CF662DA87D80A9C9763BD9C60F893087A97858C4BE80DAFBAA1518EFFF35980C` |
| `CSharpHelpers.dll` | `60BB7C17D5245B19A185CA8148C395837DA771C1CDD6DB2AF7270258EEAC7AF6` |
| `Shared.dll` | `C67E4707B9A2F570B201080DA4B1D02D9D1064BF7501F143DC985D4B2FF9AA44` |

CWTools C# tests: 18 passed, 1 skipped, 0 failed; focused parser tests: 5 passed. The broader historical F# suite built on .NET 10 but reported 32 passed, 4 skipped, and 18 failed. Those failures reproduce duplicate global-scope keys and stale rules-fixture expectations outside Irony's C# parse path; they are not caused by the identifier-character adaptation.

## Irony integration

`CWTools.Irony-Private 0.4.0-alpha12` was removed from Parser and Parser.Common. Parser now references the three reviewed binaries under `References/Direct` with explicit `HintPath` entries and declares their runtime NuGet dependencies. Parser.Common has no direct CWTools dependency. Provenance, rebuild instructions, hashes, companion binaries, and the MIT license are recorded in `References/Direct/Readme.txt` and `CWTools.LICENSE.txt`; the root README no longer requires downloading the private package.

**WORKAROUND REMOVED — modern CWTools correctly handles the syntax through Irony's real validation path.**

For Stellaris this is not a `?=` operator. The accepted grammar is identifier `starbase?` followed by ordinary `=`, with or without whitespace. The complete owner-supplied live Stellaris sample passes both `ParseScriptFile` (`IsSuccess == true`) and `CodeParser.PerformValidityCheck` (no error). Its permanent fixture is `testfiles/cwtools/stellaris-question-key.txt`. Removing its closing delimiter makes both paths reject it.

The #601-like regression workload passes on the modern stack: 250 validity parses through Irony's normal PLINQ-style path, degree capped at eight, completed with no errors or exception. This does not establish #601's cause and is not a claim that #601 is fixed.

No game cache epoch was bumped. This change affects only validity success/error selection; it does not alter serialized parser output, definition normalization, persisted conflict data, or cache schema/meaning.

## Validation

- Repository full-clean script: completed; its expected missing-directory messages produced exit 1, and verification found zero remaining `bin`/`obj` directories before restore.
- Clean Release x64 restore and full solution build: passed.
- Focused Irony CWTools tests: 10 passed.
- Parser suite: 308 total, 304 passed, 4 skipped, 0 failed.
- Seven canonical suites: 955 total, 941 passed, 14 skipped, 0 failed.
- Windows, Linux x64, and macOS x64 publish workflows: completed. Each layout contains byte-identical copies of all three direct binaries, FSharp.Core, and no `CWTools.Irony-Private` filename or dependency entry. Only Windows execution was validated; Linux/macOS validation is composition-only.
- Irony `git diff --check`: passed after documentation cleanup. CWTools fork delta relative to selected `upstream/master`: passed; full merge diff still reports whitespace already present in the imported upstream baseline.
- Performance comparison: not run; no performance improvement is claimed.

## Owner smoke gate

1. Start the normal Windows application and confirm there is no CWTools, CSharpHelpers, Shared, or FSharp.Core load error.
2. Load a representative playset and open Conflict Solver.
3. Parse a dataset containing `starbase? = { ... }`; confirm no false CWTools validity error.
4. Run a normal conflict-solver collection parse and confirm expected results.

## Recommendation

`bcssov/cwtools:irony` is the canonical CWTools source for Irony 1.28. Its lineage remains original upstream `b377453d` plus the documented narrow identifier fix and .NET 10/FSharp alignment; do not absorb the maintained fork wholesale.
