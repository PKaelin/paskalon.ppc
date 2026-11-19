# Alarms Dependencies
Services, infrastructure and libraries the Alarms solution depends on.

Back to the [Overview](./Overview.md).


## Services

| Service | Direction | How | Purpose |
|---------|-----------|-----|---------|
| [Device Service](/paskalON.DeviceService/README.md) | Consumed | REST (DER structure) via `paskalON.Devices.Client` and Redis topics (device data) | Device data to evaluate. |
| [Power Control](/paskalON.PowerControls/README.md) | Consumed | Redis topic (power targets) | Power targets to evaluate. |
| [Operating Modes](/paskalON.OperatingModes/README.md) | Consumed | Redis topic (setpoints) | Setpoints and limit reasons to evaluate. |
| [Luna](/paskalON.Luna/README.md) | Consumer | REST (Web API) and RabbitMQ (alarm actions) via `paskalON.Alarms.Client` | Alarm list, header summary and acknowledgement. |

The topics are listed in [Interfaces](./Interfaces.md).


## Infrastructure
All provided by [InfrastructureInternal](/InfrastructureInternal/README.md) in DEV and TST.

| Infrastructure | Purpose |
|----------------|---------|
| PostgreSQL (`alarms` database) | Alarm configurations, alarms, alarm action outbox and localized texts. |
| Redis | Subscribed data topics and the heartbeat of this service. |
| RabbitMQ | Exchange for the published alarm actions. |
| OpenTelemetry Collector | Logs, metrics and traces (OTLP gRPC). |


## Libraries

| Library | Solution | Purpose |
|---------|----------|---------|
| paskalON.Domains | [Common](/paskalON.Common/README.md) | Shared domain types. |
| paskalON.Telemetry | [Common](/paskalON.Common/README.md) | Metrics publisher. |
| paskalON.Hosting | [Common](/paskalON.Common/README.md) | Startup retry while dependencies are not reachable. |
| paskalON.Infrastructure | [Common](/paskalON.Common/README.md) | Generic repositories and database version. |
| paskalON.Messaging, paskalON.Messaging.Redis, paskalON.Messaging.RabbitMq | [Communication](/paskalON.Communication/README.md) | Message subscriber/publisher and their Redis and RabbitMQ implementations. |
| paskalON.Devices.Client, paskalON.Devices.Dto | [Device Service](/paskalON.DeviceService/README.md) | DER structure and device data messages. |
| paskalON.PowerControls.Dto | [Power Control](/paskalON.PowerControls/README.md) | Power target messages. |
| paskalON.OperatingModes.Dto | [Operating Modes](/paskalON.OperatingModes/README.md) | Setpoint messages. |

*Note: paskalON.Common and paskalON.Communication libraries are project references during initial development.*


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
