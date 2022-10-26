cd ..
dotnet publish src\IronyModManager.Shared\IronyModManager.Shared.csproj  /p:PublishProfile=src\IronyModManager.Shared\Properties\PublishProfiles\win-x64.pubxml --configuration Release
dotnet publish src\IronyModManager.Storage.Common\IronyModManager.Storage.Common.csproj  /p:PublishProfile=src\IronyModManager.Storage.Common\Properties\PublishProfiles\win-x64.pubxml --configuration Release
dotnet publish src\IronyModManager.IO.Common\IronyModManager.IO.Common.csproj  /p:PublishProfile=src\IronyModManager.IO.Common\Properties\PublishProfiles\win-x64.pubxml --configuration Release
dotnet publish src\IronyModManager.Models.Common\IronyModManager.Models.Common.csproj  /p:PublishProfile=src\IronyModManager.Models.Common\Properties\PublishProfiles\win-x64.pubxml --configuration Release
dotnet publish src\IronyModManager.Parser.Common\IronyModManager.Parser.Common.csproj  /p:PublishProfile=src\IronyModManager.Parser.Common\Properties\PublishProfiles\win-x64.pubxml --configuration Release
dotnet publish src\IronyModManager.Services.Common\IronyModManager.Services.Common.csproj  /p:PublishProfile=src\IronyModManager.Services.Common\Properties\PublishProfiles\win-x64.pubxml --configuration Release
dotnet publish src\IronyModManager.Storage\IronyModManager.Storage.csproj  /p:PublishProfile=src\IronyModManager.Storage\Properties\PublishProfiles\win-x64.pubxml --configuration Release
dotnet publish src\IronyModManager.Localization\IronyModManager.Localization.csproj  /p:PublishProfile=src\IronyModManager.Localization\Properties\PublishProfiles\win-x64.pubxml --configuration Release
dotnet publish src\IronyModManager.IO\IronyModManager.IO.csproj  /p:PublishProfile=src\IronyModManager.IO\Properties\PublishProfiles\win-x64.pubxml --configuration Release
dotnet publish src\IronyModManager.Models\IronyModManager.Models.csproj  /p:PublishProfile=src\IronyModManager.Models\Properties\PublishProfiles\win-x64.pubxml --configuration Release
dotnet publish src\IronyModManager.Parser\IronyModManager.Parser.csproj  /p:PublishProfile=src\IronyModManager.Parser\Properties\PublishProfiles\win-x64.pubxml --configuration Release
dotnet publish src\IronyModManager.Services\IronyModManager.Services.csproj  /p:PublishProfile=src\IronyModManager.Services\Properties\PublishProfiles\win-x64.pubxml --configuration Release
dotnet publish src\IronyModManager.DI\IronyModManager.DI.csproj  /p:PublishProfile=src\IronyModManager.DI\Properties\PublishProfiles\win-x64.pubxml --configuration Release
dotnet publish src\IronyModManager.Platform\IronyModManager.Platform.csproj  /p:PublishProfile=src\IronyModManager.Platform\Properties\PublishProfiles\win-x64.pubxml --configuration Release
dotnet publish src\IronyModManager.Common\IronyModManager.Common.csproj  /p:PublishProfile=src\IronyModManager.Common\Properties\PublishProfiles\win-x64.pubxml --configuration Release
dotnet publish src\IronyModManager.Updater\IronyModManager.Updater.csproj  /p:PublishProfile=src\IronyModManager.Updater\Properties\PublishProfiles\win-x64.pubxml --configuration Release
dotnet publish src\IronyModManager.GameLauncher\IronyModManager.GameLauncher.csproj  /p:PublishProfile=src\IronyModManager.GameLauncher\Properties\PublishProfiles\win-x64.pubxml --configuration Release
dotnet publish src\IronyModManager\IronyModManager.csproj  /p:PublishProfile=src\IronyModManager\Properties\PublishProfiles\win-x64.pubxml --configuration Release
xcopy "src\IronyModManager\bin\Release\net6.0\win-x64\*.dll" "src\IronyModManager\bin\x64\Release\net6.0\publish\win-x64\" /Y /S /D
xcopy "src\IronyModManager\bin\Release\net6.0\win-x64\*.json" "src\IronyModManager\bin\x64\Release\net6.0\publish\win-x64\" /Y /S /D
xcopy "src\IronyModManager\bin\Release\net6.0\win-x64\*.pdb" "src\IronyModManager\bin\x64\Release\net6.0\publish\win-x64\" /Y /S /D
xcopy "src\IronyModManager.Updater\bin\x64\Release\net6.0\publish\win-x64\*.*" "src\IronyModManager\bin\x64\Release\net6.0\publish\win-x64\" /Y /S /D
xcopy "src\IronyModManager.GameLauncher\bin\x64\Release\net6.0\publish\win-x64\*.*" "src\IronyModManager\bin\x64\Release\net6.0\publish\win-x64\" /Y /S /D
REM Temp fix due to avalonia bug
xcopy "%userprofile%\.nuget\packages\avalonia.angle.windows.natives\2.1.0.2020091801\runtimes\win7-x64\native\av_libglesv2.dll" "src\IronyModManager\bin\x64\Release\net6.0\publish\win-x64\" /Y /S /D
del "src\IronyModManager\bin\x64\Release\net6.0\publish\win-x64\IronyModManager.runtimeconfig.dev.json" /S /Q
del "src\IronyModManager\bin\x64\Release\net6.0\publish\win-x64\IronyModManager.Updater.runtimeconfig.dev.json" /S /Q
del "src\IronyModManager\bin\x64\Release\net6.0\publish\win-x64\IronyModManager.GameLauncher.runtimeconfig.dev.json" /S /Q
del "src\IronyModManager\bin\x64\Release\net6.0\publish\win-x64\steam_api64.dll" /S /Q
xcopy "References\CopyAll\*.*" "src\IronyModManager\bin\x64\Release\net6.0\publish\win-x64\" /Y /S /D
xcopy "References\Conditional\Steamworks\Windows-x64\*.*" "src\IronyModManager\bin\x64\Release\net6.0\publish\win-x64\" /Y /S /D
cd publish