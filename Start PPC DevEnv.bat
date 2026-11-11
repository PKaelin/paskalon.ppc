REM ----------------------------------------------------------------------------------
REM Open all relevant Visual Studio solutions and resources for the PaskalON project
REM For initial development purposes.
REM ----------------------------------------------------------------------------------
set dev="C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\devenv.exe"
set ppc=C:\Dev\GitHub\paskalon.ppc
set wait=5

REM Device Simulator
start %dev% %ppc%\paskalON.DeviceSimulator\paskalON.DeviceSimulator.slnx
timeout /t %wait%

REM Device Service
start %dev% %ppc%\paskalON.DeviceService\paskalON.DeviceService.slnx
timeout /t %wait%

REM Power Controls
start %dev% %ppc%\paskalON.PowerControls\paskalON.PowerControls.slnx
timeout /t %wait%

REM Operating Modes
start %dev% %ppc%\paskalON.OperatingModes\paskalON.OperatingModes.slnx
timeout /t %wait%

REM Alarms
start %dev% %ppc%\paskalON.Alarms\paskalON.Alarms.slnx
timeout /t %wait%

REM Luna
start %dev% %ppc%\paskalON.Luna\paskalON.Luna.slnx
timeout /t %wait%

REM Client Data
start %dev% %ppc%\paskalON.ClientData\paskalON.ClientData.slnx
timeout /t %wait%

REM Open command prompt for Client Data and show
start "Client Data" /D %ppc%\paskalON.ClientData cmd /k ImportClientData.bat

REM Launch Docker Desktop after a short delay
timeout /t 45
start "" "C:\Program Files\Docker\Docker\Docker Desktop.exe"


pause