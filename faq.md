<img src="img/logo.png" alt="Irony Mod Manager Logo" width="90" align="right">

# FAQ

## Navigation
[Home](index.md) • [Install](install.md) • [Supported Games](games.md) • [Documentation](docs.md) • [FAQ](faq.md) • [Troubleshooting](troubleshooting.md) • [Tutorials](tutorials.md)

---

<details>
<summary><strong>What load order does Irony use?</strong></summary>

Irony uses the same load order as the **Paradox Launcher v2**.  
Changes you make in Irony are applied directly to the game’s mod registry in a launcher-compatible format.

</details>

---

<details>
<summary><strong>Does drag-and-drop support multiple items?</strong></summary>

Not yet. Multi-select drag-and-drop is planned.  
Track the feature request here:  
[https://github.com/bcssov/IronyModManager/issues/12](https://github.com/bcssov/IronyModManager/issues/12)

</details>

---

<details>
<summary><strong>I modified the collection — how do I clean up the patch mod?</strong></summary>

You don’t need to do anything manually.

- Rerun the **Conflict Solver** → Irony will clean outdated patch entries  
- Or temporarily disable/delete the patch mod inside **Collection Mods**

</details>

---

<details>
<summary><strong>The conflict solver is missing for my game — why?</strong></summary>

Only **Stellaris** has full conflict solver support.

- **HOI4** supports **Analysis Only** mode  
- All other supported games use merging only  

See **Supported Games** for details.

</details>

---

<details>
<summary><strong>Editing some conflicts is difficult. Any tips?</strong></summary>

Some definitions are complex.  
Configure an external merge tool in:

**Options → External Editor**

Then use:

**Right-click → External Merge**

Tools like WinMerge, KDiff or VS Code make this much easier.

</details>

---

<details>
<summary><strong>Irony reports “invalid definitions”. Is this a problem?</strong></summary>

Invalid definitions come from CWTools and usually mean malformed or unsupported syntax.

You can:

- Inspect errors inside the **Invalid** section of Conflict Solver  
- Provide overrides via **Custom Patches**  
- Fix the mod directly  

If you think Irony flagged something incorrectly, please report it.

</details>

---

<details>
<summary><strong>Can I tell Irony to ignore a file?</strong></summary>

Yes — add one of these lines on its own in the file:

```
# Dear Irony please fallback to simple parser
```

```
# Irony this is a placeholder file please ignore it
```

Or ignore specific IDs:

```
# Irony these are placeholder objects please ignore them: id1,id2
```

</details>

---

<details>
<summary><strong>Irony does not detect Stellaris (or another game) on Linux.</strong></summary>

Some distros do not set `XDG_DATA_HOME`.  
Irony depends on it to locate Paradox game folders.

Set the variable or launch the game once to regenerate paths.

</details>

---

<details>
<summary><strong>Irony freezes or shows a black window on Linux.</strong></summary>

Possible causes:

- Avalonia UI quirks  
- Wayland incompatibility  
- Tooltip deadlocks  

Fixes:

Disable tooltips:

```
"Tooltips": { "Disable": true }
```

Or enable Wayland:

```
"LinuxOptions": { "DisplayServer": "wayland" }
```

Ensure `xwayland` is installed if using X11 fallback.

</details>

---

<details>
<summary><strong>Irony fails to launch on macOS Catalina.</strong></summary>

See:  
[https://github.com/bcssov/IronyModManager/issues/19](https://github.com/bcssov/IronyModManager/issues/19)

This is caused by Apple’s notarization and quarantine system.

</details>

---

<details>
<summary><strong>I run out of RAM when exporting/merging on macOS.</strong></summary>

macOS has a very low `ulimit` (256).  
Workaround:

1. Copy `appSettings.json` → `appSettings.override.json`  
2. Set `"UseFileStreams": true` under `"OSXOptions"`  
3. Run: `ulimit -n 200000`  
4. Launch Irony with `./IronyModManager`

Repeat step 3 for each Terminal session unless made permanent.

</details>

---

<details>
<summary><strong>Irony crashes unexpectedly or auto-update fails.</strong></summary>

Common causes:

- Antivirus blocks  
- Incomplete downloads  
- System restrictions  

Digitally signed binaries would prevent false positives but are expensive.

</details>

---

<details>
<summary><strong>Irony crashes on startup on Windows.</strong></summary>

Install Microsoft Visual C++ 2017 Redistributable:

- x86: [https://aka.ms/vs/16/release/vc_redist.x86.exe](https://aka.ms/vs/16/release/vc_redist.x86.exe)
- x64: [https://aka.ms/vs/16/release/vc_redist.x64.exe](https://aka.ms/vs/16/release/vc_redist.x64.exe)

</details>

---

<details>
<summary><strong>Can Irony upload mods to the Steam Workshop?</strong></summary>

No.  
Irony cannot upload or publish Workshop mods.

</details>

---

<details>
<summary><strong>How can I "freeze" my game state?</strong></summary>

Options:

1. **Merge → Compress**  
2. **Merge → Basic**  
3. Duplicate collection → **Export → Whole Collection** → re-import later  

This ensures all mods exist locally exactly as they were.

</details>

---

<details>
<summary><strong>Irony shows only a black window under Wayland.</strong></summary>

Fix options:

1. Install **xwayland**  
2. Set `"DisplayServer"` to `"wayland"` or `"auto"` in `appSettings.json`

</details>

---

<details>
<summary><strong>Is there a Wayland-ready package in my distro?</strong></summary>

Some distros (e.g. Arch) provide `irony-mod-manager-bin`.  
These packages are **community-maintained** and may not support Wayland.

Report issues to their maintainers, not Irony.

</details>
