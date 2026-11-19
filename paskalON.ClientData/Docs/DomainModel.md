# Client Data Domain Model
Client Data has no domain model of its own. It creates the persistent domain configuration (`*.Domain.Configs`) of the target services.

Back to the [Overview](./Overview.md).


## Created Entities
- Every line of a client project file creates one domain entity of the target service, identified by its `Type` and `Name`.
- The entity types are the configuration classes of the target services, e.g. `SystemConfig`, `ModbusConfig`, `PpcAlarmConfig`.
- References between entities are resolved by name, see [CSV File Definition](./Design/CsvFileDefinition.md).

The configuration of each target service is described in its documentation:
- [Device Service Configuration](/paskalON.DeviceService/Docs/Configuration.md)
- [Power Control Configuration](/paskalON.PowerControls/Docs/Configuration.md)
- [Operating Modes Configuration](/paskalON.OperatingModes/Docs/Configuration.md)
- [Alarms Configuration](/paskalON.Alarms/Docs/Configuration.md)
- [Device Simulator Configuration](/paskalON.DeviceSimulator/Docs/Configuration.md)


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
