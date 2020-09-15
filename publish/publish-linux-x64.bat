cd ..
dotnet publish src\IronyModManager\IronyModManager.csproj  /p:PublishProfile=src\IronyModManager\Properties\PublishProfiles\linux-x64.pubxml --configuration Release
xcopy "src\IronyModManager\bin\Release\netcoreapp3.1\linux-x64\*.dll" "src\IronyModManager\bin\Release\netcoreapp3.1\publish\linux-x64\" /Y /S /D
xcopy "References\*.*" "src\IronyModManager\bin\Release\netcoreapp3.1\publish\linux-x64\" /Y /S /D
cd publish