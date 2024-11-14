1. Avalonia.HtmlRenderer 
Source: https://github.com/bcssov/Avalonia.HtmlRenderer
Had to build due to not being able to use custom fonts
2. LiteDB
Source: https://github.com/bcssov/LiteDB
Had to build to remove hardcoded document size limit
3. NWayland & Avalonia.Wayland
Source: https://github.com/bcssov/NWayland
Check Irony branch. The functionality is still a PR and not compatible with 0.10 (only 0.11 preview). Therefore the logic was ripped from there. 
Depends on nuget: Wanhjor.ObjectInspector
4. ProDotNet 
Source: https://github.com/bcssov/ProDotNet
Built manually due to https://github.com/advisories/GHSA-xhg6-9j5j-w4vf vulnerabiity to apply a patch
Depends on nugets: System.Security.Permissions and System.Text.Encoding.CodePages