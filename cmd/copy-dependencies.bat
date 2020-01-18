@echo off

set config=%1
set outdir=%2
set solutiondir=%3
set errorlevel=0

if "%config%" == "" set errorlevel=1001
if "%outdir%" == "" set errorlevel=1001
if "%solutiondir%" == "" set errorlevel=1001
IF %errorlevel% gtr 0 exit /b %errorlevel%
	
if "%config%" == "Debug" (
	for /d %%i in ("%solutiondir%IronyModManager.*") do (
		if not "%%i" == "%solutiondir%IronyModManager" xcopy "%%i\%outdir%*.dll" "%solutiondir%IronyModManager\%outdir%" /Y /S /D
		if not "%%i" == "%solutiondir%IronyModManager" xcopy "%%i\%outdir%*.json" "%solutiondir%IronyModManager\%outdir%" /Y /S /D
		if not "%%i" == "%solutiondir%IronyModManager" xcopy "%%i\%outdir%*.pdb" "%solutiondir%IronyModManager\%outdir%" /Y /S /D
	)
)	ELSE (
	for /d %%i in ("%solutiondir%IronyModManager.*") do (
		if not "%%i" == "%solutiondir%IronyModManager" xcopy "%%i\%outdir%*.dll" "%solutiondir%IronyModManager\%outdir%" /Y /S /D
	)
)