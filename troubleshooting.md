<img src="img/logo.png" alt="Irony Mod Manager Logo" width="90" align="right">

# Troubleshooting

## Navigation
[Home](index.md) • [Install](install.md) • [Supported Games](games.md) • [Documentation](docs.md) • [FAQ](faq.md) • [Troubleshooting](troubleshooting.md) • [Tutorials](tutorials.md)

---

This page covers common issues when running Irony Mod Manager and how to report problems in a way that makes them realistically fixable.

---

# Log Files

Irony has a built-in shortcut to open the log folder.

Use the menu option:

Actions → Logs

This will open the folder where Irony stores its log files.  
When something goes wrong (crash, exception window, unexpected behaviour), grab the most recent log file from there and attach it to your issue report.

Each run of Irony produces a timestamped log. You usually only need the latest one.

---

# How to Report an Issue

Report problems on the official issue tracker:

[https://github.com/bcssov/IronyModManager/issues](https://github.com/bcssov/IronyModManager/issues)

When opening an issue, please include:

- A clear description of what you were trying to do  
- Exact steps to reproduce the problem  
- The relevant log file (via Actions → Logs)  
- Irony version  
- Game you were using and its version  

If the issue is related to mod collections:

- Do not send your full 100+ mod setup  
- Instead, create a minimal, reproducible collection that still triggers the problem  
- Export or describe that minimal collection so it can be reconstructed on another system  

Without a clear, minimal reproduction case, many issues are effectively impossible to diagnose.

---

# Common Issues & Fixes

### Irony does not launch at all

Possible causes:

- Corrupt or incomplete download  
- Antivirus or security software blocking the executable  
- Missing executable permissions on Mac/Linux  
- Files extracted to a location where your user account cannot execute them (non-Windows)

Try the following:

- Re-download and re-extract the archive  
- Temporarily whitelist Irony in your antivirus to test  
- On Unix-based systems (Mac/Linux), ensure Irony is executable using: chmod +x IronyModManager  
- Make sure you are running Irony from a location your user can read and execute (for example, your home directory)

---

### Irony launches but something behaves incorrectly

Examples:

- Applying collections does not have the expected effect  
- Conflict solver results look wrong  
- Merges are not what you expect  

In these cases:

1. Note exactly which game and which collection you are using  
2. Try to strip the setup down to the smallest number of mods that still causes the issue  
3. Save that as a test collection  
4. Open an issue and describe the behaviour, attaching:
   - The log file  
   - The description of the minimal collection (or exported data, if applicable)

---

### Conflict Solver availability

Conflict solver support is not the same for every game:

- Stellaris: full conflict solver support  
- Hearts of Iron IV: conflict analysis available, with limitations  
- Other supported games: conflict solver is not available  

See the Supported Games page for an overview of capabilities per game.

---

# Need More Help?

If you are not sure whether something is a bug or expected behaviour:

- Check the FAQ page  
- Check the Wiki for the feature in question  
- Ask in the community Discord  

Official Irony Discord:  
[https://discord.gg/t9JmY8KFrV](https://discord.gg/t9JmY8KFrV)
