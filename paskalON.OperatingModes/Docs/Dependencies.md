# Operating Modes Dependencies
Services, infrastructure and libraries the Operating Modes solution depends on.

Back to the [Overview](./Overview.md).


## Services

| Service | Direction | How | Purpose |
|---------|-----------|-----|---------|
| [Device Service](/paskalON.DeviceService/README.md) | Consumed | REST (DER structure) and Redis topics (device data) via `paskalON.Devices.Client` | Available power, measurements at the POI and DER unit states. |
| [Power Control](/paskalON.PowerControls/README.md) | Consumed | Redis topic (heartbeat) | Dependency health: the setpoints are held while the Power Control is lost. |
| [Power Control](/paskalON.PowerControls/README.md) | Consumer | Redis topics (setpoints) via `paskalON.OperatingModes.Client` | Distributes the published setpoints to the devices. |
| [Luna](/paskalON.Luna/README.md) | Consumer | REST (Web API) and Redis topics (setpoints) | Operator user interface for the stack, overrides and setpoints. |
| [Alarms](/paskalON.Alarms/README.md) | Consumer | Redis topics (setpoints) | Evaluates alarms on the published setpoints. |

The topics are listed in [Interfaces](./Interfaces.md).


## Infrastructure
All provided by [InfrastructureInternal](/InfrastructureInternal/README.md) in DEV and TST.

| Infrastructure | Purpose |
|----------------|---------|
| PostgreSQL (`operatingmodes` database) | System and operating mode configuration. |
| Redis | Message broker for the published and subscribed topics. A lost connection holds the setpoints. |
| OpenTelemetry Collector | Logs, metrics and traces (OTLP gRPC). |


## Libraries

| Library | Solution | Purpose |
|---------|----------|---------|
| paskalON.Domains | [Common](/paskalON.Common/README.md) | Shared domain types (e.g. power control type). |
| paskalON.Maths | [Common](/paskalON.Common/README.md) | Interpolation and other calculations for curves and ramps. |
| paskalON.PhysicalUnits | [Common](/paskalON.Common/README.md) | Physical quantities (active power, reactive power, voltage, frequency). |
| paskalON.Telemetry | [Common](/paskalON.Common/README.md) | Metrics publisher. |
| paskalON.Hosting | [Common](/paskalON.Common/README.md) | Startup retry while dependencies are not reachable. |
| paskalON.Infrastructure | [Common](/paskalON.Common/README.md) | Generic repositories and database version. |
| paskalON.Messaging | [Communication](/paskalON.Communication/README.md) | Message publisher/subscriber abstractions, heartbeats, sequencing and dependency health. |
| paskalON.Messaging.Redis | [Communication](/paskalON.Communication/README.md) | Redis implementation of the message publisher/subscriber. |
| paskalON.Devices.Client | [Device Service](/paskalON.DeviceService/README.md) | Client to receive the DER structure and device data. |

*Note: paskalON.Common and paskalON.Communication libraries are project references during initial development.*


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
