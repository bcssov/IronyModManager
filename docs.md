<img src="img/logo.png" alt="Irony Mod Manager Logo" width="90" align="right">

# Documentation Hub

## Navigation
[Home](index.md) • [Install](install.md) • [Supported Games](games.md) • [Documentation](docs.md) • [FAQ](faq.md) • [Troubleshooting](troubleshooting.md) • [Tutorials](tutorials.md)

---

Welcome to the central documentation hub for Irony Mod Manager.  
This page provides **one-click access** to all official documentation resources.

---

# 📘 Full Wiki Documentation

The complete and authoritative Irony documentation is hosted on the Wiki:

👉 **[https://github.com/bcssov/IronyModManager/wiki](https://github.com/bcssov/IronyModManager/wiki)**

The Wiki includes:

- General application usage  
- Load order logic  
- Conflict solver documentation (Stellaris only)  
- Mod merging  
- Game-specific behavior  
- Privacy policy  
- User guides and discussions  

---

# Exact Mod Set Ignore Rules

Irony 1.28 can ignore a conflict only when its participating mods are exactly equal to a configured set. For example, `modSet:"A","B"` matches A + B or B + A, but not A alone or A + B + C. All names are quoted; inside a name, `\"` escapes a double quote and `\\` escapes a backslash.

The Conflict Solver manager lists existing rules and supports **Add**, **Delete**, and **Close**. Analysis mode provides a read-only **Preview only** view with Add and Delete unavailable. Matching uses canonical mod names rather than local aliases.

These rules are new in 1.28. Older Irony versions do not understand `modSet:` correctly; normal round-trips preserve the text, but older releases should not be relied on to evaluate or manage these rules.

See the authoritative [Ignore Rules guide](https://github.com/bcssov/IronyModManager/wiki/Ignore-Rules) for complete syntax, examples, and compatibility details.

---

# 🧭 New User Checklist

If you're using Irony for the first time, start here:

👉 **[https://github.com/bcssov/IronyModManager/wiki/New-User-Checklist](https://github.com/bcssov/IronyModManager/wiki/New-User-Checklist)**

This checklist provides a quick overview of the essential steps for creating a collection and preparing your game.

---

# 📄 PDF Documentation

A downloadable PDF manual is available here:

👉 **[https://github.com/bcssov/IronyModManager/discussions/210](https://github.com/bcssov/IronyModManager/discussions/210)**

It is generated from the Wiki and updated regularly.

---

# 🌍 Translations

If you want to help with translations or report issues with existing ones:

👉 **[https://github.com/bcssov/IronyModManager/discussions/231](https://github.com/bcssov/IronyModManager/discussions/231)**

Irony uses community-driven localization with externalized resource files.

---

# 🔐 Privacy Policy

The privacy policy is available on the Wiki:

👉 **[Privacy Policy](https://github.com/bcssov/IronyModManager/wiki/Privacy-Policy)**

---

# 📞 Additional Resources

If you're looking for troubleshooting steps or encountered an issue:

- See the [Troubleshooting](troubleshooting.md) page  
- Check the [FAQ](faq.md)  
- Or report issues directly on GitHub:  
  👉 [https://github.com/bcssov/IronyModManager/issues](https://github.com/bcssov/IronyModManager/issues)
