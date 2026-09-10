@echo off

set config=%~1
set outdir=%~2
set solutiondir=%~3

if "%config%" == "" exit /b 1001
if "%outdir%" == "" exit /b 1001
if "%solutiondir%" == "" exit /b 1001

set destination=%solutiondir%src\IronyModManager\%outdir%
set manifest=%solutiondir%src\IronyModManager\obj\composition-%config%.txt

call :validate_projects %*
if errorlevel 1 exit /b 1002
if exist "%manifest%" for /f "usebackq delims=" %%f in ("%manifest%") do if exist "%%f" del /q "%%f"
type nul > "%manifest%"

call :copy_projects %*

xcopy "%solutiondir%References\CopyAll\*.*" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D
xcopy "%solutiondir%References\Conditional\Steamworks\Windows-x64\*.*" "%solutiondir%src\IronyModManager\%outdir%" /Y /S /D
goto :eof

:validate_projects
shift
shift
shift
:validate_project
if "%~1" == "" exit /b 0
if not exist "%solutiondir%src\%~1\%outdir%%~1.dll" exit /b 1002
shift
goto validate_project

:copy_projects
shift
shift
shift
:copy_next_project
if "%~1" == "" exit /b 0
call :copy_project "%~1"
shift
goto copy_next_project

:copy_project
xcopy "%solutiondir%src\%~1\%outdir%*.dll" "%destination%" /Y /S /D
xcopy "%solutiondir%src\%~1\%outdir%*.exe" "%destination%" /Y /S /D
xcopy "%solutiondir%src\%~1\%outdir%*.json" "%destination%" /Y /S /D
xcopy "%solutiondir%src\%~1\%outdir%*.pdb" "%destination%" /Y /S /D
xcopy "%solutiondir%src\%~1\%outdir%*.dylib" "%destination%" /Y /S /D
xcopy "%solutiondir%src\%~1\%outdir%*.so" "%destination%" /Y /S /D
for %%e in (dll pdb deps.json) do if exist "%destination%%~1.%%e" echo %destination%%~1.%%e>>"%manifest%"
exit /b 0
