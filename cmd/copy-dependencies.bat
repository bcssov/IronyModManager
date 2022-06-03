@echo off

set config=%1
set outdir=%2
set solutiondir=%3
set errorlevel=0

set copydir=%outdir:linux-x64\=%
set copydir=%copydir:osx-x64\=%
set copydir=%copydir:win-x64\=%

if "%config%" == "" set errorlevel=1001
if "%outdir%" == "" set errorlevel=1001
if "%solutiondir%" == "" set errorlevel=1001
IF %errorlevel% gtr 0 exit /b %errorlevel%

if "%config%" == "Release" (
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
) else (
	for /d %%i in ("%solutiondir%src\IronyModManager.*") do (
		set "projPath=%%i"
		setlocal enabledelayedexpansion	
		if "!projPath:Tests=!"=="!projPath!" (
			if "!projPath:Updater=!"=="!projPath!" (
				if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%copydir%*.dll" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D
				if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%copydir%*.exe" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D
				if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%copydir%*.json" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D
				if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%copydir%*.pdb" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D
				if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%copydir%*.dylib" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D
				if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%copydir%*.so" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D				
			) else (
				if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%outdir%*.dll" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D
				if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%outdir%*.exe" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D
				if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%outdir%*.json" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D
				if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%outdir%*.pdb" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D			
				if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%outdir%*.dylib" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D			
				if not "%%i" == "%solutiondir%src\IronyModManager" xcopy "%%i\%outdir%*.so" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D			
			)
		)
		endlocal
	)
)
xcopy "%solutiondir%References\*.*" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D