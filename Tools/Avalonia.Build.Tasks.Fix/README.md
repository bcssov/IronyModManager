# Avalonia.Build.Tasks 0.10.22 sharing-violation workaround

This is an optional source workaround for an old Avalonia/XamlX build-task
sharing violation. It is not part of Irony's runtime architecture, normal
repository setup, or normal build flow.

The symptom is a `CompileAvaloniaXamlTask` failure that mentions `original.pdb`
and reports that a process cannot access a file because another process is using
it. It was rarely observed during normal Visual Studio 2022 development, but is
frequent enough under Visual Studio 2026 with Irony's pinned Avalonia 0.10.22
toolchain to justify preserving this maintainer workaround.

Irony 1.28 remains on the Avalonia 0.10.22 compatibility boundary. Related
upstream history: [AvaloniaUI/Avalonia#5294](https://github.com/AvaloniaUI/Avalonia/issues/5294)
and [AvaloniaUI/Avalonia#12670](https://github.com/AvaloniaUI/Avalonia/issues/12670).
Retire this tool when Irony moves to a modern Avalonia generation where the
workaround is no longer needed.

## What it changes

The tool uses Mono.Cecil to find `XamlX.TypeSystem.CecilTypeSystem`, its
parameterless `Dispose`, and each `newobj` construction site. Each construction
must be immediately stored in a local. The tool inserts the original
maintainer-script transformation before every `ret` in that method:

```il
ldloc <CecilTypeSystem local>
callvirt instance void XamlX.TypeSystem.CecilTypeSystem::Dispose()
ret
```

The unmodified Avalonia 0.10.22 task assembly has exactly one such construction
site and three return sites, so the patch inserts exactly three calls. Any other
shape is rejected rather than being treated as compatible.

## Use

Obtain the unmodified `Avalonia.Build.Tasks.dll` from the `Avalonia` 0.10.22
NuGet package in your normal global-packages cache. `dotnet nuget locals
global-packages --list` prints the cache root; the package DLL is under:

```text
<global-packages>\avalonia\0.10.22\tools\netstandard2.0\Avalonia.Build.Tasks.dll
```

Build and run the tool with explicit paths. Choose an output outside the NuGet
cache first; the tool never overwrites its input and does not modify the cache
by default.

```powershell
dotnet run --project Tools\Avalonia.Build.Tasks.Fix\src\Avalonia.Build.Tasks.Fix\Avalonia.Build.Tasks.Fix.csproj -- `
  --input "<global-packages>\avalonia\0.10.22\tools\netstandard2.0\Avalonia.Build.Tasks.dll" `
  --output "<chosen-output-directory>\Avalonia.Build.Tasks.dll"
```

Use `--inspect` with `--input` to report the discovered structure without
writing an output. The tool recognizes an assembly it previously patched and
copies it unchanged to the explicit output location; a partially patched
assembly is rejected.

If the workaround is needed, close Visual Studio and active builds, retain a
copy of the original package DLL, and manually copy the generated output over
the matching DLL in your local NuGet cache. This is a per-machine temporary
workaround, not a repository change. To restore the original package, remove
the affected `avalonia\0.10.22` global-packages entry and restore again (or
restore the retained original DLL).

Never commit a patched DLL, package-cache contents, or other generated binary.
