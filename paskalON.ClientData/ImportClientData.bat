@echo off
rem -----------------------------------------------------------------------------
rem Imports the client data CSV file of a client project into the microservice databases.
rem
rem Usage: ImportClientData.bat ClientProjectName Service[:SchemaVersion] [Service[:SchemaVersion] ...]
rem   ClientProjectName  Name of the client project and of its CSV file without extension.
rem   Service            DS  = DeviceService   (paskalON.Devices.Data)
rem                      SIM = DeviceSimulator (paskalON.DeviceSimulator.Data)
rem                      PC  = PowerControls   (paskalON.PowerControls.Data)
rem                      OM  = OperatingModes  (paskalON.OperatingModes.Data)
rem                      PPC = DS, SIM, PC and OM
rem   SchemaVersion      Optional migration id of that service, the latest version is used without it.
rem                      Not allowed for PPC because every service has its own migrations.
rem
rem Examples:
rem   ImportClientData.bat DemoSuperSimpleBattery PPC
rem   ImportClientData.bat DemoSuperSimpleBattery DS:20261020230224_v_0_1 SIM
rem
rem Important: The database of every selected service is deleted and recreated.
rem All arguments, applications and CSV files are checked before the first database is touched.
rem The services run in the order DS, SIM, PC, OM and a failed service does not stop the others.
rem Set CLIENTDATA_DRY_RUN=1 to only show the commands without executing them.
rem
rem Exit codes: 0 = success, 2 = invalid arguments or missing application or CSV file,
rem             otherwise the exit code of the first failed import (see README.md).
rem -----------------------------------------------------------------------------
setlocal EnableExtensions EnableDelayedExpansion

rem ---- Configuration ----------------------------------------------------------
set "ROOT=%~dp0"
set "BUILD_DIR=bin\Debug\net10.0"

rem Client data console applications
set "SIM_EXE=%ROOT%paskalON.DeviceSimulator.Data\%BUILD_DIR%\paskalON.DeviceSimulator.Data.exe"
set "DS_EXE=%ROOT%paskalON.Devices.Data\%BUILD_DIR%\paskalON.Devices.Data.exe"
set "PC_EXE=%ROOT%paskalON.PowerControls.Data\%BUILD_DIR%\paskalON.PowerControls.Data.exe"
set "OM_EXE=%ROOT%paskalON.OperatingModes.Data\%BUILD_DIR%\paskalON.OperatingModes.Data.exe"

rem Root directories of the client data CSV files, searched including sub directories
set "SIM_CSV_ROOT=%ROOT%paskalON.DeviceSimulator.Data\ClientData"
set "DS_CSV_ROOT=%ROOT%paskalON.Devices.Data\ClientData"
set "PC_CSV_ROOT=%ROOT%paskalON.PowerControls.Data\ClientData"
set "OM_CSV_ROOT=%ROOT%paskalON.OperatingModes.Data\ClientData"

rem Display names
set "SIM_NAME=DeviceSimulator"
set "DS_NAME=DeviceService"
set "PC_NAME=PowerControls"
set "OM_NAME=OperatingModes"
rem -----------------------------------------------------------------------------

if "%~1"=="" goto :usage
if "%~2"=="" goto :usage

set "PROJECT=%~1"
set "SCRIPT=%~nx0"
shift

:parse
if "%~1"=="" goto :check

set "SERVICE="
set "SCHEMA="

for /f "tokens=1,* delims=:" %%a in ("%~1") do (
    set "SERVICE=%%a"
    set "SCHEMA=%%b"
)

if /i "!SERVICE!"=="PPC" if defined SCHEMA goto :ppcschema

if /i "!SERVICE!"=="PPC" (
    for %%s in (DS SIM PC OM) do set "%%s_RUN=1"
) else if /i "!SERVICE!"=="SIM" (
    call :select SIM
) else if /i "!SERVICE!"=="DS" (
    call :select DS
) else if /i "!SERVICE!"=="PC" (
    call :select PC
) else if /i "!SERVICE!"=="OM" (
    call :select OM
) else (
    echo Unknown service argument: %~1
    goto :usage
)

shift
goto :parse

:check
rem Check everything before the first database is deleted
for %%s in (DS SIM PC OM) do (
    call :resolve %%s
    if errorlevel 1 exit /b 2
)

set "EXIT_CODE=0"

for %%s in (DS SIM PC OM) do (
    call :import %%s

    if errorlevel 1 if "!EXIT_CODE!"=="0" set "EXIT_CODE=!errorlevel!"
)

echo.

if "!EXIT_CODE!"=="0" (
    echo All selected imports of %PROJECT% succeeded.
) else (
    echo At least one import of %PROJECT% failed, first exit code: !EXIT_CODE!
)

exit /b !EXIT_CODE!


rem -----------------------------------------------------------------------------
rem Selects a service with its optional schema version.
rem %1 = service key
rem -----------------------------------------------------------------------------
:select
set "%1_RUN=1"
set "%1_SCHEMA=!SCHEMA!"
exit /b 0


rem -----------------------------------------------------------------------------
rem Checks the application and finds exactly one CSV file of the project.
rem %1 = service key
rem -----------------------------------------------------------------------------
:resolve
if not defined %1_RUN exit /b 0

set "EXE=!%1_EXE!"
set "CSV_ROOT=!%1_CSV_ROOT!"
set "FOUND="
set "COUNT=0"

if not exist "!EXE!" (
    echo [%1] Application not found: !EXE!
    echo [%1] Build paskalON.ClientData.slnx first.
    exit /b 1
)

if not exist "!CSV_ROOT!\" (
    echo [%1] CSV root directory not found: !CSV_ROOT!
    exit /b 1
)

for /f "delims=" %%f in ('dir /s /b /a-d "!CSV_ROOT!\%PROJECT%.csv" 2^>nul') do (
    set /a COUNT+=1
    set "FOUND=%%f"
)

if !COUNT! equ 0 (
    echo [%1] No file %PROJECT%.csv found in !CSV_ROOT! or its sub directories.
    exit /b 1
)

if !COUNT! gtr 1 (
    echo [%1] More than one file %PROJECT%.csv found, the client project name must be unique:
    dir /s /b /a-d "!CSV_ROOT!\%PROJECT%.csv"
    exit /b 1
)

set "%1_FILE=!FOUND!"
exit /b 0


rem -----------------------------------------------------------------------------
rem Runs the client data application of a service.
rem %1 = service key
rem -----------------------------------------------------------------------------
:import
if not defined %1_RUN exit /b 0

set "ARGS="%PROJECT%" "!%1_FILE!""

if defined %1_SCHEMA set ARGS=!ARGS! "!%1_SCHEMA!"

echo.
echo ==== !%1_NAME!: "!%1_EXE!" !ARGS!

if "%CLIENTDATA_DRY_RUN%"=="1" exit /b 0

"!%1_EXE!" !ARGS!
set "CODE=!errorlevel!"

if not "!CODE!"=="0" echo [%1] !%1_NAME! import failed with exit code !CODE!.

exit /b !CODE!


:ppcschema
echo PPC does not accept a schema version, use DS:Version SIM:Version PC:Version OM:Version instead.
exit /b 2


:usage
if not defined SCRIPT set "SCRIPT=%~nx0"
echo Usage: !SCRIPT! ClientProjectName Service[:SchemaVersion] [Service[:SchemaVersion] ...]
echo   Service: DS, SIM, PC, OM or PPC for all of them
echo   Example: !SCRIPT! DemoSuperSimpleBattery PPC
echo   Example: !SCRIPT! DemoSuperSimpleBattery DS:20261020230224_v_0_1 SIM
exit /b 2
