@echo off
cd ..
for /d %%i in (src\IronyModManager*) do (
	RD /s /q "%%i\bin"
	RD /s /q "%%i\obj"
)
RD /s /q "TestResults"