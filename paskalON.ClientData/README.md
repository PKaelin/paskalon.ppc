# Client Data
Loads client data or sample data into the microservices databases.

Purpose, how it works and the full documentation of the solution are in the [Client Data Overview](./Docs/Overview.md).


## Structure

| Project | Purpose |
|---------|---------|
| [paskalON.ClientData.Common](./paskalON.ClientData.Common/README.md) | CSV parsing, schema version selection, property conversion and reference resolution. |
| [paskalON.Devices.Data](./paskalON.Devices.Data/README.md) | Imports the client data of the Device Service. |
| [paskalON.DeviceSimulator.Data](./paskalON.DeviceSimulator.Data/README.md) | Imports the client data of the Device Simulator. |
| [paskalON.PowerControls.Data](./paskalON.PowerControls.Data/README.md) | Imports the client data of the Power Control. |
| [paskalON.OperatingModes.Data](./paskalON.OperatingModes.Data/README.md) | Imports the client data of the Operating Modes service. |
| [paskalON.Alarms.Data](./paskalON.Alarms.Data/README.md) | Imports the client data of the Alarms service. |

*Note: paskalON.Common libraries are project references during initial development.*


## Import
Build the solution, start [InfrastructureInternal](/InfrastructureInternal/README.md) and run:
```
ImportClientData.bat DemoSuperSimpleBattery PPC
```
**Important:** the database of every selected service is deleted and recreated. The arguments and exit codes are described in [Interfaces](./Docs/Interfaces.md).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
