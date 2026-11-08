1. Avalonia.HtmlRenderer 
Source: https://github.com/bcssov/Avalonia.HtmlRenderer
Had to build due to not being able to use custom fonts
2. LiteDB
Source: https://github.com/bcssov/LiteDB (irony branch)
Maintained Irony fork: raises unsuitable hardcoded limits and carries the extended index-key encoding required by Irony.
3. NWayland & Avalonia.Wayland
Source: https://github.com/bcssov/NWayland
Use the irony branch, which is authoritative for Irony's maintained fork. Avalonia.Wayland is a custom backend/backport deliberately kept compatible with Avalonia 0.10.22 and deployed with NWayland.dll as direct references. The fork includes maintained compatibility and lifecycle fixes, with generated Wayland protocol bindings intentionally committed.
Depends on nuget: Wanhjor.ObjectInspector
4. CWTools, CSharpHelpers & Shared
Maintainer fork: https://github.com/bcssov/cwtools, branch: irony
Original lineage: https://github.com/cwtools/cwtools
Modern comparison source: https://github.com/Aa728848/cwtools
Built from bcssov/cwtools:irony commit 0b550b0d6f56da60411b34ffc4df3ca27a0c3243: cwtools/cwtools baseline b377453dee803f9258be92cfc49896d09039702d, the identifier-character fix adapted from Aa728848/cwtools commit 21e48dfd242d31caa77c314cb5d59de5cd7db28b, and the reviewed .NET 10/FSharp.Core 10.1.400 alignment.
Irony carries direct binaries because no suitable current upstream NuGet package exposes the reviewed parser build. CWTools.dll has runtime assembly dependencies on CSharpHelpers.dll and Shared.dll, so all three must stay together.
Rebuild: checkout bcssov/cwtools:irony, restore CWTools/CWTools.fsproj, then run `dotnet build CWTools/CWTools.fsproj -c Release`. Copy the three DLLs from artifacts/bin/<project>/release into this directory and update their hashes below.
Binary SHA-256:
CWTools.dll CF662DA87D80A9C9763BD9C60F893087A97858C4BE80DAFBAA1518EFFF35980C
CSharpHelpers.dll 60BB7C17D5245B19A185CA8148C395837DA771C1CDD6DB2AF7270258EEAC7AF6
Shared.dll C67E4707B9A2F570B201080DA4B1D02D9D1064BF7501F143DC985D4B2FF9AA44
License: MIT; preserve CWTools.LICENSE.txt with redistributed binaries.
