# Client Data Dependencies
Target services, infrastructure and libraries the Client Data solution depends on.

Back to the [Overview](./Overview.md).


## Target Services
Client Data writes the configuration through the infrastructure projects of the target services:

| Data project | Target service | Infrastructure project | Database |
|--------------|----------------|------------------------|----------|
| paskalON.Devices.Data | [Device Service](/paskalON.DeviceService/README.md) | paskalON.Devices.Infrastructure | `devices` |
| paskalON.DeviceSimulator.Data | [Device Simulator](/paskalON.DeviceSimulator/README.md) | paskalON.Devices.Infrastructure | `devicesimulator` |
| paskalON.PowerControls.Data | [Power Control](/paskalON.PowerControls/README.md) | paskalON.PowerControls.Infrastructure | `powercontrols` |
| paskalON.OperatingModes.Data | [Operating Modes](/paskalON.OperatingModes/README.md) | paskalON.OperatingModes.Infrastructure | `operatingmodes` |
| paskalON.Alarms.Data | [Alarms](/paskalON.Alarms/README.md) | paskalON.Alarms.Infrastructure | `alarms` |


## Infrastructure
All provided by [InfrastructureInternal](/InfrastructureInternal/README.md) in DEV and TST.

| Infrastructure | Purpose |
|----------------|---------|
| PostgreSQL | The databases of the target services. |


## Libraries

| Library | Solution | Purpose |
|---------|----------|---------|
| paskalON.Domains | [Common](/paskalON.Common/README.md) | Shared domain types. |


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
