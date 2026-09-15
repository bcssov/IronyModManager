<img src="img/logo.png" alt="Irony Mod Manager Logo" width="90" align="right">

# Installation & Run Instructions

## Navigation
[Home](index.md) • [Install](install.md) • [Supported Games](games.md) • [Documentation](docs.md) • [FAQ](faq.md) • [Troubleshooting](troubleshooting.md) • [Tutorials](tutorials.md)

---

Irony Mod Manager is distributed as portable builds for all platforms, as an installer for Windows, and as an unsigned Finder convenience bundle for macOS.

If you're unsure which file to download, follow the guide below.

---

# Which file should I download?

## Windows
- Portable version: win-x64.zip
- Installer version: win-x64-setup.zip

## MacOS
- Finder convenience bundle (recommended): `IronyModManager-osx-x64-app.tar.gz`
- Portable fallback: `osx-x64.zip`

## Linux
- Portable version: linux-x64.zip

---

# How to run Irony

## Windows

### Portable build
1. Download win-x64.zip
2. Extract it anywhere
3. Run `IronyModManager.exe`

### Installer build
1. Download win-x64-setup.zip
2. Extract it
3. Run `win-x64-setup.exe`
4. Follow on-screen instructions

---

## MacOS

### Finder convenience bundle (recommended)
1. Download `IronyModManager-osx-x64-app.tar.gz`.
2. Extract it using a method that preserves Unix executable permissions.
3. Open `IronyModManager.app` from Finder.
4. Complete the normal macOS approval flow if prompted for downloaded unsigned software.

The bundle is unsigned and unnotarized. It is a Finder convenience bundle, not a native installer. The 1.28 bundle has been verified to launch from Finder on macOS 11.

### Portable fallback
1. Download and extract `osx-x64.zip`.
2. Open Terminal in the extracted directory.
3. Make the file executable: `chmod +x IronyModManager`.
4. Run Irony: `./IronyModManager`.

### Updates
Irony continues to check for updates on macOS, but it does not install them automatically or execute the downloaded updater helper. When an update is available, **Open Release Page** opens the official GitHub Releases page. Download the preferred macOS build and replace the existing copy manually.

---

## Linux
1. Download and extract linux-x64.zip
2. Open a terminal in the extracted directory
3. Make Irony executable: `chmod +x IronyModManager`
4. Run Irony: `./IronyModManager`

---

# Windows 7 Support
Windows 7 is no longer officially supported.  
For community findings and workarounds, see:  
[https://github.com/bcssov/IronyModManager/discussions/469](https://github.com/bcssov/IronyModManager/discussions/469)  

---

# Need help?
If Irony fails to start, check the Troubleshooting page for common fixes and log locations.
