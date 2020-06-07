Irony Mod Manager is a new mod manager for Paradox Games which at the moment supports Stellaris only but at a later date support for additional games will be added. The games which will be supported are the ones which support the newest Paradox Launcher.

### Download
* Latest stable version: [v1.0.133](https://github.com/bcssov/IronyModManager/releases/tag/v1.0.133).
* Latest prerelease version: [v1.1.84-alpha](https://github.com/bcssov/IronyModManager/releases/tag/v1.1.84-alpha).

### 1.1 migration notes
* The 1.1 version holds significant changes over 1.0 and is still a work in progress. 1.1 version will migrate database.json (file which holds all of your settings, collections and so on) of 1.0 and will switch to using its own database.json file called database_1.1.json. If you keep using 1.0 and 1.1 in sync please be aware that any changes in 1.1 will not be reflected in 1.0 and vice-versa. You can export\import your collections though into and from 1.0\1.1. If you'd like to reset Irony 1.1 you can always delete database_1.1.json file and Irony will reimport the 1.0 version file again.
* Collection patch mods are not backwards compatible, if you run Irony conflict solver it will not be backwards compatible with 1.0 version. While 1.0 version will know how to import it, running conflict solver over a collection modified by 1.1 will result in 1.0 rejecting some incompatible exports made by 1.1 version

### New users
Check this [checklist](https://github.com/bcssov/IronyModManager/wiki/New-User-Checklist) if you are a first time user of Irony.

### Documentation
Documentation is hosted on the [wiki](https://github.com/bcssov/IronyModManager/wiki).

### Issues\Bugs
Reports issues and bugs [here](https://github.com/bcssov/IronyModManager/issues). 
* Be sure to include as much detailed information as you can.
* Be sure to include steps to reproduce the issue.
* Be sure to provide logs if you report crashes. Logs can be found in the logs directory of your Irony install.

### Notes
**OSX:**
Extract the .7z file to a location you want Irony installed to. Open Terminal.app and cd to the directory you installed to. Run ```chmod +x IronyModManager```. That will allow you to execute the app. Then you can either start the app in Terminal.app with the command ```./IronyModManger``` (the ./ are important!) or double-click the file in Finder.

**Linux:**
Extract the 7z file somewhere that you'd like to have Irony installed. Navigate to the folder where you extracted Irony and run the ```chmod +x IronyModManager``` command. That will allow you to execute the app. Finally you can run the app using the ```./IronyModManager``` command.
