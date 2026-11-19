# Operating Modes Interfaces
APIs, messaging topics and client library exposed or consumed by the Operating Modes solution.

Back to the [Overview](./Overview.md).


## Web API
Base address in DEV: `http://localhost:45020`. Requests are in the REST Client file [paskalON.OperatingModes.Service.http](../paskalON.OperatingModes.Service/paskalON.OperatingModes.Service.http).


### Stack (`/api/v1/stack/...`)

| Method | Action | Description |
|--------|--------|-------------|
| GET | `getavailableoperatingmodes` | Gets all stackable operating modes that can be added to the stack. |
| GET | `getactiveoperatingmodes` | Gets the operating modes of the stack in ascending priority order. |
| POST | `addoperatingmode` | Adds an operating mode to the stack with the specified name and priority. |
| POST | `moveoperatingmode` | Moves an operating mode of the stack to a new priority. |
| POST | `removeoperatingmode` | Removes an operating mode from the stack. |
| POST | `replaceoperatingmode` | Replaces an operating mode of the stack with another one at the same priority in one atomic change. |
| POST | `setoperatingmodes` | Sets the whole stack in one atomic change. Only the resulting stack is validated. |


### Override (`/api/v1/override/...`)

| Method | Action | Description |
|--------|--------|-------------|
| GET | `getactiveoperatingmodes` | Gets the active override modes. |
| POST | `activateoperatingmode` | Activates an override mode. |
| POST | `deactivateoperatingmode` | Deactivates an override mode. |
| POST | `setsetpoint` | Sets the operator setpoints of a maintenance override mode. |


### System (`/api/v1/system/...`)

| Method | Action | Description |
|--------|--------|-------------|
| GET | `getsetpoint` | Gets the operator setpoints of the system. |
| POST | `setsetpoint` | Sets the operator setpoints of the system. |
| POST | `startallpcs` | Starts all PCS that are not in maintenance mode. |
| POST | `standbyallpcs` | Puts all PCS that are not in maintenance mode into standby. |
| POST | `stopallpcs` | Stops all PCS that are not in maintenance mode. |


### Version (`/api/v1/version`)

| Method | Action | Description |
|--------|--------|-------------|
| GET | – | Gets the latest database version. |

Request bodies are defined in `paskalON.OperatingModes.Service.Dto`, response bodies in `paskalON.OperatingModes.Dto`.


## Published Topics (Redis)
The topic names are configured in the [system configuration](./Configuration.md#system-configuration). The values below are from the demo client data.

| Configuration property | Demo topic | Message | Description |
|------------------------|------------|---------|-------------|
| `PublisherTopicCurrentSetpoint` | `ppc:om:setpoints` | `CurrentSetpointsDto` | Applied plant-level setpoints, supervisory setpoints and limit reasons. Republished periodically even if unchanged. |
| `PublisherTopicCurrentDerUnitSetpoint` | `ppc:om:setpoints:unit` | `CurrentDerUnitSetpointsDto` | Setpoints of override modes for single DER units. |
| `PublisherTopicHeartbeat` | `ppc:om:heartbeat` | `HeartbeatDto` | Heartbeat of this service. |

All messages carry `SourceInstance`, `Sequence` and `Timestamp`, so receivers can drop outdated or duplicate messages.


## Subscribed Topics (Redis)

| Configuration property | Demo topic | Publisher | Description |
|------------------------|------------|-----------|-------------|
| `SubscriberTopicPcsCore` / `...Detail` | `ppc:device:pcs:core` / `:detail` | Device Service | Power conversion system data. |
| `SubscriberTopicBatteryBankCore` / `...Detail` | `ppc:device:bb:core` / `:detail` | Device Service | Battery bank data. |
| `SubscriberTopicSolarPanelCore` / `...Detail` | `ppc:device:pv:core` / `:detail` | Device Service | Solar panel data. |
| `SubscriberTopicSystemPowerMeterCore` / `...Detail` | `ppc:device:pm:system:core` / `:detail` | Device Service | Power meter at the POI (feedback of closed-loop modes). |
| `SubscriberTopicExternalPowerMeterCore` / `...Detail` | `ppc:device:pm:external:core` / `:detail` | Device Service | External power meter data. |
| `SubscriberTopicAuxiliaryPowerMeterCore` / `...Detail` | `ppc:device:pm:auxiliary:core` / `:detail` | Device Service | Auxiliary power meter data. |
| `SubscriberTopicCircuitPowerMeterCore` / `...Detail` | `ppc:device:pm:circuit:core` / `:detail` | Device Service | Circuit power meter data. |
| `SubscriberTopicPowerControlHeartbeat` | `ppc:pc:heartbeat` | Power Control | Heartbeat for the dependency health. |


## Client Library
[paskalON.OperatingModes.Client](../paskalON.OperatingModes.Client/README.md) subscribes to the setpoint topics for other services. Each consuming service implements `IPowerSetpointHandler`.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
