# Documentation
Check the [Wiki](https://github.com/bcssov/IronyModManager/wiki)

# Building Irony Mod Manager
All instructions are for Windows.
1. Install Visual Studio 2022 (required for .NET6)
1. Clone the repo to your local machine
1. Open the LocalizationResourceGenerator solution file located in \Tools\LocalizationResourceGenerator\src
    * Build the solution and copy the binaries to the Tools\LocalizationResourceGenerator folder
1. If you don't already have one, create a folder for local NuGet packages and unzip the [CWTools.Irony-Private.0.4.0-alpha4](https://github.com/bcssov/IronyModManager/files/6292686/CWTools.Irony-Private.0.4.0-alpha4.zip) package to it
    * This is just an up to date version of CWTools, the one on the public NuGet is older
    * Example path: C:\Users\username\code\LocalNuGet
    * If you need to set up a local NuGet repo:
        1. If not already registered, register the LocalNuGet folder with VisualStudio by clicking Tools -> NuGet Package Manager -> Package Manager Settings
        1. Select the Package Sources menu item
        1. Hit the '+' plus sign icon to create a new package source
            1. Name it Local
            1. Point it at the folder you created before
1. In the IronyModManager directory, create a folder called "keys"
    * Example path: C:\Users\username\code\IronyModManager\keys
1. Open the Visual Studio Terminal and create the following keys in that folder by using the command "sn -k keyPairName.snk"
    * Irony-Main
    * Irony-Plugin
1. In the terminal, unpack the public keys from the key pairs by using the command "sn -p keyPairName.snk publicKeyName.snk"
    * Irony-Main-Public
    * Irony-Plugin-Public
1. Copy the public keys to the \src\IronyModManager.DI folder
1. Right click the IronyModManager.Parser project file and select Manage NuGet Packages
    * CWTools has a dependency on FSharp.Core v4.7.0 that isn't automatically resolved, so add that to the project
1. Restore NuGet Packages for the solution
1. Rebuild All
1. Set IronyModManager as the Startup project and launch it

# Special Thanks
Special thanks to tboby from [CWTools](https://github.com/tboby/cwtools) for extending CWTools API for my needs. And also thanks to all early adopters and testers.