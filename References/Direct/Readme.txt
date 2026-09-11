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
