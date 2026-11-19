# Luna Dependencies
Services, infrastructure and libraries the Luna solution depends on.

Back to the [Overview](./Overview.md).


## Services

| Service | Direction | How | Purpose |
|---------|-----------|-----|---------|
| [Device Service](/paskalON.DeviceService/README.md) | Consumed | REST and Redis topics via `paskalON.Devices.Client` | DER structure, device values and device commands. |
| [Operating Modes](/paskalON.OperatingModes/README.md) | Consumed | REST and Redis topics (setpoints, heartbeat) via `paskalON.OperatingModes.Client` | Operating mode stack, overrides, setpoints and setpoint status. |
| [Power Control](/paskalON.PowerControls/README.md) | Consumed | Redis topics (power targets, heartbeat) via `paskalON.PowerControls.Client` | Power targets and dependency status. |
| [Alarms](/paskalON.Alarms/README.md) | Consumed | REST, RabbitMQ (alarm actions) and Redis (heartbeat) via `paskalON.Alarms.Client` | Alarm list, alarm summary, acknowledgement and localized texts. |
| Grafana | Consumed by Luna Web | Embedded dashboards | Metrics dashboards. |

The topics are listed in [Interfaces](./Interfaces.md).


## Infrastructure
All provided by [InfrastructureInternal](/InfrastructureInternal/README.md) in DEV and TST.

| Infrastructure | Purpose |
|----------------|---------|
| Redis | Subscribed topics of the backend services. |
| RabbitMQ | Alarm actions of the alarm service. |
| OpenTelemetry Collector | Logs, metrics and traces (OTLP gRPC). |
| Grafana | Dashboards embedded in Luna Web. |


## Libraries

| Library | Solution | Purpose |
|---------|----------|---------|
| paskalON.PhysicalUnits | [Common](/paskalON.Common/README.md) | Physical quantities. |
| paskalON.Telemetry | [Common](/paskalON.Common/README.md) | Metrics publisher. |
| paskalON.Hosting | [Common](/paskalON.Common/README.md) | Startup retry while dependencies are not reachable. |
| paskalON.Messaging, paskalON.Messaging.Redis | [Communication](/paskalON.Communication/README.md) | Message subscriber, heartbeats and dependency health. |
| paskalON.Devices.Client | [Device Service](/paskalON.DeviceService/README.md) | DER structure, device data and commands. |
| paskalON.OperatingModes.Client, paskalON.OperatingModes.Dto | [Operating Modes](/paskalON.OperatingModes/README.md) | Setpoints and stack DTOs. |
| paskalON.PowerControls.Client, paskalON.PowerControls.Dto | [Power Control](/paskalON.PowerControls/README.md) | Power targets. |
| paskalON.Alarms.Client | [Alarms](/paskalON.Alarms/README.md) | Alarm API and alarm actions. |

Luna Web uses Vue 3, TypeScript, Vite, Bootstrap 5 and the SignalR JavaScript client, see [paskalON.Luna.Web](../paskalON.Luna.Web/README.md).

*Note: paskalON.Common and paskalON.Communication libraries are project references during initial development.*


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
