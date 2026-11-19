# Power Control Interfaces
APIs, messaging topics and client library exposed or consumed by the Power Control solution.

Back to the [Overview](./Overview.md).


## Web API
Base address in DEV: `http://localhost:45010`. Requests are in the REST Client file [paskalON.PowerControls.Service.http](../paskalON.PowerControls.Service/paskalON.PowerControls.Service.http).

| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/v1/system/setpowersetpoints` | Sets the power setpoints of the system. |
| POST | `/api/v1/system/setderunitpowersetpoints` | Sets the power setpoints of a DER unit (the unit has to be in maintenance). |
| POST | `/api/v1/system/startpcs` | Starts a specific PCS identified by its device ID. |
| POST | `/api/v1/system/startallpcs` | Starts all PCS that are not in maintenance mode. |
| POST | `/api/v1/system/stopallpcs` | Stops all PCS that are not in maintenance mode. |
| POST | `/api/v1/system/standbyallpcs` | Puts all PCS that are not in maintenance mode into standby. |
| GET | `/api/v1/version` | Gets the latest database version. |

Request bodies are defined in `paskalON.PowerControls.Service.Dto`.


## Published Topics (Redis)
The topic names are configured in the [system configuration](./Configuration.md#system-configuration). The values below are from the demo client data.

| Configuration property | Demo topic | Message | Description |
|------------------------|------------|---------|-------------|
| `PublisherTopicPowerTarget` | `ppc:pc:targets` | `CurrentTargetsDto` | Power targets of the PCS. |
| `PublisherTopicHeartbeat` | `ppc:pc:heartbeat` | `HeartbeatDto` | Heartbeat of this service. |


## Subscribed Topics (Redis)

| Configuration property | Demo topic | Publisher | Description |
|------------------------|------------|-----------|-------------|
| `SubscriberTopicCurrentSetpoint` | `ppc:om:setpoints` | Operating Modes | System setpoints. |
| `SubscriberTopicCurrentDerUnitSetpoint` | `ppc:om:setpoints:unit` | Operating Modes | DER unit setpoints of override modes. |
| `SubscriberTopicPcsCore` / `...Detail` | `ppc:device:pcs:core` / `:detail` | Device Service | Power conversion system data. |
| `SubscriberTopicBatteryBankCore` / `...Detail` | `ppc:device:bb:core` / `:detail` | Device Service | Battery bank data. |
| `SubscriberTopicSolarPanelCore` / `...Detail` | `ppc:device:pv:core` / `:detail` | Device Service | Solar panel data. |
| `SubscriberTopicSystemPowerMeterCore` / `...Detail` | `ppc:device:pm:system:core` / `:detail` | Device Service | System power meter data. |
| `SubscriberTopicCircuitPowerMeterCore` / `...Detail` | `ppc:device:pm:circuit:core` / `:detail` | Device Service | Circuit power meter data. |
| `SubscriberTopicExternalPowerMeterCore` / `...Detail` | `ppc:device:pm:external:core` / `:detail` | Device Service | External power meter data. |
| `SubscriberTopicAuxiliaryPowerMeterCore` / `...Detail` | `ppc:device:pm:auxiliary:core` / `:detail` | Device Service | Auxiliary power meter data. |


## Client Library
[paskalON.PowerControls.Client](../paskalON.PowerControls.Client/README.md) subscribes to the power targets for other services. Each consuming service implements `IPowerTargetHandler`.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
