cd ..
dotnet build --configuration Release
dotnet publish src\IronyModManager\IronyModManager.csproj  /p:PublishProfile=src\IronyModManager\Properties\PublishProfiles\win-x64.pubxml --configuration Release
xcopy "src\IronyModManager\bin\Release\netcoreapp3.1\win-x64\*.dll" "src\IronyModManager\bin\Release\netcoreapp3.1\publish\win-x64\" /Y /S /D
cd publish