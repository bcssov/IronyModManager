cd ..
dotnet publish src\IronyModManager.Shared\IronyModManager.Shared.csproj  /p:PublishProfile=src\IronyModManager.Shared\Properties\PublishProfiles\osx-x64.pubxml --configuration osx-x64
dotnet publish src\IronyModManager.Storage.Common\IronyModManager.Storage.Common.csproj  /p:PublishProfile=src\IronyModManager.Storage.Common\Properties\PublishProfiles\osx-x64.pubxml --configuration osx-x64
dotnet publish src\IronyModManager.IO.Common\IronyModManager.IO.Common.csproj  /p:PublishProfile=src\IronyModManager.IO.Common\Properties\PublishProfiles\osx-x64.pubxml --configuration osx-x64
dotnet publish src\IronyModManager.Models.Common\IronyModManager.Models.Common.csproj  /p:PublishProfile=src\IronyModManager.Models.Common\Properties\PublishProfiles\osx-x64.pubxml --configuration osx-x64
dotnet publish src\IronyModManager.Parser.Common\IronyModManager.Parser.Common.csproj  /p:PublishProfile=src\IronyModManager.Parser.Common\Properties\PublishProfiles\osx-x64.pubxml --configuration osx-x64
dotnet publish src\IronyModManager.Services.Common\IronyModManager.Services.Common.csproj  /p:PublishProfile=src\IronyModManager.Services.Common\Properties\PublishProfiles\osx-x64.pubxml --configuration osx-x64
dotnet publish src\IronyModManager.Storage\IronyModManager.Storage.csproj  /p:PublishProfile=src\IronyModManager.Storage\Properties\PublishProfiles\osx-x64.pubxml --configuration osx-x64
dotnet publish src\IronyModManager.Localization\IronyModManager.Localization.csproj  /p:PublishProfile=src\IronyModManager.Localization\Properties\PublishProfiles\osx-x64.pubxml --configuration osx-x64
dotnet publish src\IronyModManager.IO\IronyModManager.IO.csproj  /p:PublishProfile=src\IronyModManager.IO\Properties\PublishProfiles\osx-x64.pubxml --configuration osx-x64
dotnet publish src\IronyModManager.Models\IronyModManager.Models.csproj  /p:PublishProfile=src\IronyModManager.Models\Properties\PublishProfiles\osx-x64.pubxml --configuration osx-x64
dotnet publish src\IronyModManager.Parser\IronyModManager.Parser.csproj  /p:PublishProfile=src\IronyModManager.Parser\Properties\PublishProfiles\osx-x64.pubxml --configuration osx-x64
dotnet publish src\IronyModManager.Services\IronyModManager.Services.csproj  /p:PublishProfile=src\IronyModManager.Services\Properties\PublishProfiles\osx-x64.pubxml --configuration osx-x64
dotnet publish src\IronyModManager.DI\IronyModManager.DI.csproj  /p:PublishProfile=src\IronyModManager.DI\Properties\PublishProfiles\osx-x64.pubxml --configuration osx-x64
dotnet publish src\IronyModManager.Platform\IronyModManager.Platform.csproj  /p:PublishProfile=src\IronyModManager.Platform\Properties\PublishProfiles\osx-x64.pubxml --configuration osx-x64
dotnet publish src\IronyModManager.Common\IronyModManager.Common.csproj  /p:PublishProfile=src\IronyModManager.Common\Properties\PublishProfiles\osx-x64.pubxml --configuration osx-x64
dotnet publish src\IronyModManager.Updater\IronyModManager.Updater.csproj  /p:PublishProfile=src\IronyModManager.Updater\Properties\PublishProfiles\osx-x64.pubxml --configuration osx-x64
dotnet publish src\IronyModManager.GameHandler\IronyModManager.GameHandler.csproj  /p:PublishProfile=src\IronyModManager.GameHandler\Properties\PublishProfiles\osx-x64.pubxml --configuration osx-x64
dotnet publish src\IronyModManager\IronyModManager.csproj  /p:PublishProfile=src\IronyModManager\Properties\PublishProfiles\osx-x64.pubxml --configuration osx-x64
xcopy "src\IronyModManager\bin\osx-x64\net6.0\osx-x64\*.dll" "src\IronyModManager\bin\x64\osx-x64\net6.0\publish\osx-x64\" /Y /S /D
xcopy "src\IronyModManager\bin\osx-x64\net6.0\osx-x64\*.json" "src\IronyModManager\bin\x64\osx-x64\net6.0\publish\osx-x64\" /Y /S /D
xcopy "src\IronyModManager\bin\osx-x64\net6.0\osx-x64\*.pdb" "src\IronyModManager\bin\x64\osx-x64\net6.0\publish\osx-x64\" /Y /S /D
xcopy "src\IronyModManager.Updater\bin\x64\osx-x64\net6.0\publish\osx-x64\*.*" "src\IronyModManager\bin\x64\osx-x64\net6.0\publish\osx-x64\" /Y /S /D
xcopy "src\IronyModManager.GameHandler\bin\x64\osx-x64\net6.0\publish\osx-x64\*.*" "src\IronyModManager\bin\x64\osx-x64\net6.0\publish\osx-x64\" /Y /S /D
del "src\IronyModManager\bin\x64\osx-x64\net6.0\publish\osx-x64\IronyModManager.runtimeconfig.dev.json" /S /Q
del "src\IronyModManager\bin\x64\osx-x64\net6.0\publish\osx-x64\IronyModManager.Updater.runtimeconfig.dev.json" /S /Q
del "src\IronyModManager\bin\x64\osx-x64\net6.0\publish\osx-x64\IronyModManager.GameHandler.runtimeconfig.dev.json" /S /Q
del "src\IronyModManager\bin\x64\osx-x64\net6.0\publish\osx-x64\steam_api64.dll" /S /Q
xcopy "References\CopyAll\*.*" "src\IronyModManager\bin\x64\osx-x64\net6.0\publish\osx-x64\" /Y /S /D
REM Why on earth cannot nuget include these? Also the documentation sucks in this regard
xcopy "References\Conditional\Steamworks\OSX-Linux-x64\steam_api.bundle\Contents\MacOS\*.*" "src\IronyModManager\bin\x64\osx-x64\net6.0\publish\osx-x64\" /Y /S /D
cd publish