# Device Service Dependencies
Devices, services, infrastructure and libraries the Device Service solution depends on.

Back to the [Overview](./Overview.md).


## Devices

| Device | Protocol | Purpose |
|--------|----------|---------|
| Power conversion systems (PCS), battery banks, solar | Modbus TCP | Read device data, write power targets and commands. |
| Power meters (system, circuit, external, auxiliary) | IEEE C37.118 | Measurements at the POI and in the plant. |

In DEV and TST the devices are simulated by the [Device Simulator](/paskalON.DeviceSimulator/README.md).


## Services

| Service | Direction | How | Purpose |
|---------|-----------|-----|---------|
| [Power Control](/paskalON.PowerControls/README.md) | Consumed | Redis topic (power targets) via `paskalON.PowerControls.Client` | Power targets of the PCS. |
| [Operating Modes](/paskalON.OperatingModes/README.md) | Consumer | REST and Redis topics via `paskalON.Devices.Client` | Available power, measurements and DER unit states. |
| [Power Control](/paskalON.PowerControls/README.md) | Consumer | REST and Redis topics via `paskalON.Devices.Client` | Device data for the power distribution. |
| [Alarms](/paskalON.Alarms/README.md) | Consumer | Redis topics via `paskalON.Devices.Client` | Evaluates alarms on the device data. |
| [Luna](/paskalON.Luna/README.md) | Consumer | REST and Redis topics via `paskalON.Devices.Client` | Shows the devices and sends device commands. |

The topics are listed in [Interfaces](./Interfaces.md).


## Infrastructure
All provided by [InfrastructureInternal](/InfrastructureInternal/README.md) in DEV and TST.

| Infrastructure | Purpose |
|----------------|---------|
| PostgreSQL (`devices` database) | System, DER structure and device configuration. |
| Redis | Message broker for the published and subscribed topics. |
| OpenTelemetry Collector | Logs, metrics and traces (OTLP gRPC). |


## Libraries

| Library | Solution | Purpose |
|---------|----------|---------|
| paskalON.Dataface | [Common](/paskalON.Common/README.md) | Dataface registration, decouples the domain from the communication. |
| paskalON.Domains | [Common](/paskalON.Common/README.md) | Shared domain types. |
| paskalON.Maths | [Common](/paskalON.Common/README.md) | Calculations used by the configuration. |
| paskalON.PhysicalUnits | [Common](/paskalON.Common/README.md) | Physical quantities. |
| paskalON.Telemetry | [Common](/paskalON.Common/README.md) | Metrics publisher. |
| paskalON.Infrastructure | [Common](/paskalON.Common/README.md) | Generic repositories and database version. |
| paskalON.Messaging, paskalON.Messaging.Redis | [Communication](/paskalON.Communication/README.md) | Message publisher/subscriber and its Redis implementation. |
| paskalON.Protocols.Modbus | [Communication](/paskalON.Communication/README.md) | Modbus TCP client. |
| paskalON.Protocols.C37118 | [Communication](/paskalON.Communication/README.md) | IEEE C37.118 synchrophasor protocol. |
| paskalON.PowerControls.Client | [Power Control](/paskalON.PowerControls/README.md) | Subscriber for the power targets. |

*Note: paskalON.Common and paskalON.Communication libraries are project references during initial development.*


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
