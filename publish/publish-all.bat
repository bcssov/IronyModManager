cd ..
cd cmd
call clean-solution-full.bat
cd src/Irony.AppCastGenerator
dotnet build --configuration Release
cd ..
cd ..
cd publish
call publish-linux-x64.bat
call publish-osx-x64.bat
call publish-win-x64.bat
call publish-win-x64-setup.bat