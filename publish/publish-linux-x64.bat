cd ..
dotnet build --configuration Release
dotnet publish src\IronyModManager\IronyModManager.csproj  /p:PublishProfile=src\IronyModManager\Properties\PublishProfiles\linux-x64.pubxml --configuration Release
xcopy "src\IronyModManager\bin\Release\netcoreapp3.1\linux-x64\*.dll" "src\IronyModManager\bin\Release\netcoreapp3.1\publish\linux-x64\" /Y /S /D
del "src\IronyModManager\bin\Release\netcoreapp3.1\publish\linux-x64\*.pdb" /S
del "src\IronyModManager\bin\Release\netcoreapp3.1\publish\linux-x64\*.xml" /S