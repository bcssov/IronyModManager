@echo off
cd ..
for /d %%i in (src\IronyModManager*) do (
	RD /s /q "%%i\bin"
	RD /s /q "%%i\obj"
)
for /d %%i in (src\Irony.*) do (
	RD /s /q "%%i\bin"
	RD /s /q "%%i\obj"
)
RD /s /q "TestResults"
RD /s /q Tools\LocalizationResourceGenerator\src\LocalizationResourceGenerator\bin
RD /s /q Tools\LocalizationResourceGenerator\src\LocalizationResourceGenerator\obj