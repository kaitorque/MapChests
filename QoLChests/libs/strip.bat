@echo off
SETLOCAL ENABLEDELAYEDEXPANSION
pushd %~dp0
set "failed_files=echo failed files: "
set startTime=%time%
set /A "files=0"
set /A "failed=0"
set /p location=C:\Program Files\Unity\Hub\Editor\2020.3.12f1\Editor\Data\MonoBleedingEdge\lib\mono\4.5\mono-cil-strip.exe

for /r %%v in (*.dll) do (
"C:\Program Files\Unity\Hub\Editor\2020.3.12f1\Editor\Data\MonoBleedingEdge\lib\mono\4.5\mono-cil-strip.exe" %%v
)

echo Start Time: %startTime%
echo Finish Time: %time%
echo Files: %files%
IF NOT "%failed%"=="0" (echo failed: %failed% && %failed_files%)
pause