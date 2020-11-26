@echo off
cd ..
cd Tools\LocalizationResourceGenerator\src
dotnet build --configuration Release
cd ..
cd ..
cd ..
xcopy "Tools\LocalizationResourceGenerator\src\LocalizationResourceGenerator\bin\Release\net5.0\*.dll" "Tools\LocalizationResourceGenerator\" /Y /S /D
xcopy "Tools\LocalizationResourceGenerator\src\LocalizationResourceGenerator\bin\Release\net5.0\*.exe" "Tools\LocalizationResourceGenerator\" /Y /S /D
xcopy "Tools\LocalizationResourceGenerator\src\LocalizationResourceGenerator\bin\Release\net5.0\*.json" "Tools\LocalizationResourceGenerator\" /Y /S /D