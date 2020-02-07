@echo off

set config=%1
set outdir=%2
set solutiondir=%3
set errorlevel=0

set copydir=%outdir:linux-x64\=%
set copydir=%copydir:osx-x64\=%
set copydir=%copydir:win-x64\=%
set copydir=%copydir:win-x86\=%

if "%config%" == "" set errorlevel=1001
if "%outdir%" == "" set errorlevel=1001
if "%solutiondir%" == "" set errorlevel=1001
IF %errorlevel% gtr 0 exit /b %errorlevel%

if "%config%" == "Debug" (
	for /d %%i in ("%solutiondir%src\IronyModManager.*") do (
        set "projPath=%%i"
        setlocal enabledelayedexpansion	
		if "!projPath:Tests=!"=="!projPath!" (
			if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%copydir%*.dll" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D
			if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%copydir%*.json" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D
			if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%copydir%*.pdb" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D
		)
		endlocal
	)
)	ELSE (
	for /d %%i in ("%solutiondir%src\IronyModManager.*") do (
		set "projPath=%%i"
        setlocal enabledelayedexpansion	
		if "!projPath:Tests=!"=="!projPath!" (
			if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%copydir%*.dll" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D		
		)
		endlocal
	)
)