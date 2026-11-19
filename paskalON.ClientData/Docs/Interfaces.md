# Client Data Interfaces
Input arguments, import script and exit codes of the Client Data applications.

Back to the [Overview](./Overview.md).


## Input Arguments
Each of the Data projects takes the following arguments:


| Input Argument | Description |
|----------------|-------------|
| ClientProjectName | The name of the client project (E.g. DemoSuperSimpleBattery, DemoSuperSimpleSolar, ZuerichNorth, etc.) |
| ClientProjectFile | The path to the client project file (E.g. C:\Projects\DemoSuperSimpleBattery\DemoSuperSimpleBattery.csv) |
| SchemaVersion | The version of the schema to use (E.g. 1.0, 2.0, etc. or if empty it will take the latest version) |


## Import Script
[ImportClientData.bat](../ImportClientData.bat) runs the Data projects of one or more services for a client project:
```
ImportClientData.bat ClientProjectName Service[:SchemaVersion] [Service[:SchemaVersion] ...]
```

| Service | Data project |
|---------|--------------|
| `DS` | paskalON.Devices.Data |
| `SIM` | paskalON.DeviceSimulator.Data |
| `PC` | paskalON.PowerControls.Data |
| `OM` | paskalON.OperatingModes.Data |
| `AL` | paskalON.Alarms.Data |
| `PPC` | DS, SIM, PC, OM and AL (no schema version allowed) |

Examples:
```
ImportClientData.bat DemoSuperSimpleBattery PPC
ImportClientData.bat DemoSuperSimpleBattery DS:20261020230224_v_0_1 SIM
```

- All arguments, applications and CSV files are checked before the first database is touched.
- The services run in the order DS, SIM, PC, OM, AL and a failed service does not stop the others.
- Set `CLIENTDATA_DRY_RUN=1` to only show the commands without executing them.
- The script uses the Debug build output (`bin\Debug\net10.0`), so build the solution first.


## Application Exit Codes
Application exit codes are used to indicate the result of the execution of the Data project. 
The following table describes the exit codes and their meanings. `ImportClientData.bat` returns 2 for invalid arguments or a missing application or CSV file, otherwise the exit code of the first failed import.

| Exit code | Meaning |
|-----------|---------|
| 0 | Successfully imported |
| 1 | General execution error |
| 2 | Invalid arguments |
| 3 | CSV validation error |
| 4 | Database error |


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
