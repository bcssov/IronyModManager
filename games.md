<img src="img/logo.png" alt="Irony Mod Manager Logo" width="90" align="right">

# Supported Games & Feature Matrix

## Navigation
[Home](index.md) • [Install](install.md) • [Supported Games](games.md) • [Documentation](docs.md) • [FAQ](faq.md) • [Troubleshooting](troubleshooting.md) • [Tutorials](tutorials.md)

---

Irony Mod Manager supports a wide range of Paradox games.  
However, the level of support varies depending on the game's engine, file structure, and modding ecosystem.

Below is the **accurate and authoritative support matrix** for Irony.

---

# ✔ Support Matrix

| Game                | Mod Management | Conflict Solver     | Mod Merging |
|--------------------|----------------|----------------------|-------------|
| Crusader Kings III | Yes            | No                   | Yes         |
| Europa Universalis IV | Yes        | No                   | Yes         |
| Europa Universalis V | Yes        | No                   | Yes         |
| Hearts of Iron IV  | Yes            | Analysis Only        | Yes         |
| Imperator: Rome    | Yes            | No                   | Yes         |
| Star Trek: Infinite | Yes           | No                   | Yes         |
| Stellaris          | Yes            | Full                 | Yes         |
| Victoria 3         | Yes            | No                   | Yes         |

---

# What the categories mean

### **Mod Management**
Irony can fully manage the game's mod list, load order, and metadata.

### **Conflict Solver**
How deeply Irony can analyze and resolve mod conflicts:

- **No** — Conflict solver is not available for this game  
- **Analysis Only** — Can detect conflicts but cannot apply automated resolutions  
- **Full** — Complete conflict solver support (unique to Stellaris)

### **Mod Merging**
Irony can create merged mods or auto-generate merge output for compatible formats.

---

# Notes on Differences Between Games

- **Stellaris** has the most advanced support due to deep integration and long-term tooling availability.  
- **HOI4** uses a file structure that allows conflict *detection*, but not all forms of auto-resolution.  
- **CK3, EU4, V3, Imperator, STI** currently do not support the conflict solver, but mod management and merging work.  
- All supported games benefit from deterministic load order management and metadata fixes.

---

For more technical details, visit the **Irony Wiki**:  
[https://github.com/bcssov/IronyModManager/wiki](https://github.com/bcssov/IronyModManager/wiki)
