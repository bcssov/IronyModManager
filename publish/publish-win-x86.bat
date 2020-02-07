cd ..
dotnet build --configuration Release
dotnet publish src\IronyModManager\IronyModManager.csproj  /p:PublishProfile=src\IronyModManager\Properties\PublishProfiles\win-x86.pubxml --configuration Release
xcopy "src\IronyModManager\bin\Release\netcoreapp3.1\win-x86\*.dll" "src\IronyModManager\bin\Release\netcoreapp3.1\publish\win-x86\" /Y /S /D
del "src\IronyModManager\bin\Release\netcoreapp3.1\publish\win-x86\*.pdb" /S
del "src\IronyModManager\bin\Release\netcoreapp3.1\publish\win-x86\*.xml" /S