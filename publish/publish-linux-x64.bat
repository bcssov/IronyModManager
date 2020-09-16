cd ..
dotnet publish src\IronyModManager\IronyModManager.csproj  /p:PublishProfile=src\IronyModManager\Properties\PublishProfiles\linux-x64.pubxml --configuration Release
xcopy "src\IronyModManager\bin\Release\netcoreapp3.1\linux-x64\*.dll" "src\IronyModManager\bin\Release\netcoreapp3.1\publish\linux-x64\" /Y /S /D
xcopy "src\IronyModManager\bin\Release\netcoreapp3.1\linux-x64\*.exe" "src\IronyModManager\bin\Release\netcoreapp3.1\publish\linux-x64\" /Y /S /D
xcopy "src\IronyModManager\bin\Release\netcoreapp3.1\linux-x64\*.json" "src\IronyModManager\bin\Release\netcoreapp3.1\publish\linux-x64\" /Y /S /D
xcopy "src\IronyModManager\bin\Release\netcoreapp3.1\linux-x64\*.pdb" "src\IronyModManager\bin\Release\netcoreapp3.1\publish\linux-x64\" /Y /S /D
del "src\IronyModManager\bin\Release\netcoreapp3.1\publish\linux-x64\IronyModManager.runtimeconfig.dev.json" /S /Q
del "src\IronyModManager\bin\Release\netcoreapp3.1\publish\linux-x64\IronyModManager.Updater.runtimeconfig.dev.json" /S /Q
xcopy "References\*.*" "src\IronyModManager\bin\Release\netcoreapp3.1\publish\linux-x64\" /Y /S /D
cd publish