# Power Control Dependencies
Services, infrastructure and libraries the Power Control solution depends on.

Back to the [Overview](./Overview.md).


## Services

| Service | Direction | How | Purpose |
|---------|-----------|-----|---------|
| [Operating Modes](/paskalON.OperatingModes/README.md) | Consumed | Redis topics (setpoints) via `paskalON.OperatingModes.Client` | System and DER unit setpoints to distribute. |
| [Device Service](/paskalON.DeviceService/README.md) | Consumed | REST (DER structure) and Redis topics (device data) via `paskalON.Devices.Client` | DER units, nameplates and device states for the distribution and constraints. |
| [Device Service](/paskalON.DeviceService/README.md) | Consumer | Redis topic (power targets) via `paskalON.PowerControls.Client` | Writes the power targets to the PCS. |
| [Operating Modes](/paskalON.OperatingModes/README.md) | Consumer | Redis topic (heartbeat) | Holds its setpoints while the Power Control is lost. |
| [Alarms](/paskalON.Alarms/README.md) | Consumer | Redis topic (power targets) | Evaluates alarms on the power targets. |
| [Luna](/paskalON.Luna/README.md) | Consumer | Redis topic (power targets) via `paskalON.PowerControls.Client` | Shows the power targets. |

The topics are listed in [Interfaces](./Interfaces.md).


## Infrastructure
All provided by [InfrastructureInternal](/InfrastructureInternal/README.md) in DEV and TST.

| Infrastructure | Purpose |
|----------------|---------|
| PostgreSQL (`powercontrols` database) | System, power control and constraint configuration. |
| Redis | Message broker for the published and subscribed topics. |
| OpenTelemetry Collector | Logs, metrics and traces (OTLP gRPC). |


## Libraries

| Library | Solution | Purpose |
|---------|----------|---------|
| paskalON.Domains | [Common](/paskalON.Common/README.md) | Shared domain types. |
| paskalON.PhysicalUnits | [Common](/paskalON.Common/README.md) | Physical quantities. |
| paskalON.Telemetry | [Common](/paskalON.Common/README.md) | Metrics publisher. |
| paskalON.Hosting | [Common](/paskalON.Common/README.md) | Startup retry while dependencies are not reachable. |
| paskalON.Infrastructure | [Common](/paskalON.Common/README.md) | Generic repositories and database version. |
| paskalON.Messaging, paskalON.Messaging.Redis | [Communication](/paskalON.Communication/README.md) | Message publisher/subscriber, heartbeats and their Redis implementation. |
| paskalON.OperatingModes.Client | [Operating Modes](/paskalON.OperatingModes/README.md) | Subscriber for the setpoints. |
| paskalON.Devices.Client | [Device Service](/paskalON.DeviceService/README.md) | Client for the DER structure and device data. |

*Note: paskalON.Common and paskalON.Communication libraries are project references during initial development.*


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
