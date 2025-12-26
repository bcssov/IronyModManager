# Irony Mod Manager

Irony Mod Manager is a moddable, high‑performance mod manager primarily focused on Paradox games (e.g., Stellaris, EU4, HOI4). It replaces the built‑in Paradox launchers with something faster, more reliable, and actually aware of how mods interact with each other.

---

## 🔽 Download

Always download the latest version here:

👉 **[Latest Irony Mod Manager Release](https://github.com/bcssov/IronyModManager/releases/latest)**

Official Irony Mod Manager binaries are published on GitHub Releases.  
Package managers such as winget and AUR source their downloads from there.

---

## Documentation

Full documentation is available on the project Wiki:

- **[Irony Mod Manager Wiki](https://github.com/bcssov/IronyModManager/wiki)**

The Wiki covers:

- Installation and basic usage  
- Supported games  
- Load order, conflict resolution and advanced features  
- Troubleshooting and known issues  

---

## Building Irony Mod Manager (Windows)

All instructions below are for **Windows** and **Visual Studio 2022**.

### Prerequisites

- Windows  
- **[Visual Studio 2022](https://visualstudio.microsoft.com/)** with the *.NET 8* workload  
- Git  
- Command line (CMD or PowerShell)  

---

## Step‑by‑step build guide

### 1. Install Visual Studio 2022  
Make sure the .NET 8 SDK workload is installed.

### 2. Clone the repository

```bash
git clone https://github.com/bcssov/IronyModManager.git
cd IronyModManager
```

### 3. Build the LocalizationResourceGenerator

```bat
cmd\build-tools.bat
```

### 4. Set up the local CWTools NuGet package

Irony uses a newer CWTools build than the public NuGet package.

1. Create a folder for local NuGet packages, for example:

   ```text
   C:\Users\username\code\LocalNuGet
   ```

2. Download and unzip the private CWTools build:

   **[CWTools.Irony-Private.0.4.0-alpha8](https://github.com/bcssov/IronyModManager/files/7798143/CWTools.Irony-Private.0.4.0-alpha8.zip)**  
   into that folder.

3. (Optional) Add the *LocalNuGet* folder as a package source:  
   Visual Studio → Tools → NuGet Package Manager → Package Manager Settings → Package Sources.

---

### 5. Create strong name keys

Create a folder:

```text
C:\Users\username\code\IronyModManager\keys
```

Open the *Visual Studio Developer Command Prompt* and run:

```bat
sn -k Irony-Main.snk
sn -k Irony-Plugin.snk

sn -p Irony-Main.snk Irony-Main-Public.snk
sn -p Irony-Plugin.snk Irony-Plugin-Public.snk
```

Copy:

- `Irony-Main-Public.snk`  
- `Irony-Plugin-Public.snk`  

into:

```text
.\src\IronyModManager.DI
```

---

### 6. Fix CWTools FSharp.Core dependency

CWTools depends on `FSharp.Core` v4.7.0.

In Visual Studio:

- Right‑click **IronyModManager.Parser**  
- Select **Manage NuGet Packages**  
- Install **FSharp.Core 4.7.0**

---

### 7. Restore NuGet packages  
Build once and let Visual Studio restore dependencies automatically.

### 8. Build the solution  
Visual Studio → Build → Rebuild Solution.

### 9. Run Irony Mod Manager  
Set **IronyModManager** as the Startup Project → press **F5**.

If everything is configured correctly, Irony Mod Manager will launch.

---

## Special Thanks

Special thanks to **tboby** for extending the CWTools API for Irony:  
**[CWTools on GitHub](https://github.com/tboby/cwtools)**

Thanks also to all early adopters and testers for their continued feedback and support.
