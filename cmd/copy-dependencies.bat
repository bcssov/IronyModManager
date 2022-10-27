@echo off

set config=%1
set outdir=%2
set solutiondir=%3
set errorlevel=0

if "%config%" == "" set errorlevel=1001
if "%outdir%" == "" set errorlevel=1001
if "%solutiondir%" == "" set errorlevel=1001
if %errorlevel% gtr 0 exit /b %errorlevel%

for /d %%i in ("%solutiondir%src\IronyModManager.*") do (
	set "projPath=%%i"
	setlocal enabledelayedexpansion	
	if "!projPath:Tests=!"=="!projPath!" (
		if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%outdir%*.dll" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D
		if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%outdir%*.exe" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D
		if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%outdir%*.json" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D
		if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%outdir%*.pdb" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D			
		if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%outdir%*.dylib" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D			
		if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%outdir%*.so" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D			
	)
	endlocal
)

xcopy "%solutiondir%References\CopyAll\*.*" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D
xcopy "%solutiondir%References\Conditional\Steamworks\Windows-x64\*.*" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D