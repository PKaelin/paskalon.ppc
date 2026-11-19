# Device Service Interfaces
APIs, messaging topics and client library exposed or consumed by the Device Service solution.

Back to the [Overview](./Overview.md).


## Web API
Base address in DEV: `http://localhost:45000`. Requests are in the REST Client file [paskalON.Devices.Service.http](../paskalON.Devices.Service/paskalON.Devices.Service.http).

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/v1/der/getder` | Gets the DER root with all its contents (groups, units, circuits, devices). |
| POST | `/api/v1/der/setmaintenance` | Puts a DER unit into or out of maintenance mode. |
| POST | `/api/v1/pcs/startall` | Starts all PCS that are not in maintenance mode. |
| POST | `/api/v1/pcs/stopall` | Stops all PCS that are not in maintenance mode. |
| POST | `/api/v1/pcs/standbyall` | Puts all PCS that are not in maintenance mode into standby. |
| POST | `/api/v1/pcs/start` | Starts a specific PCS. |
| POST | `/api/v1/pcs/stop` | Stops a specific PCS. |
| POST | `/api/v1/pcs/standby` | Puts a specific PCS into standby. |
| POST | `/api/v1/pcs/setpowertarget` | Sets the power target of a specific PCS. |
| POST | `/api/v1/bb/connect` | Connects a battery bank. |
| POST | `/api/v1/bb/disconnect` | Disconnects a battery bank. |
| POST | `/api/v1/powermeter/connectall` | Connects all power meters that are not already connected. |
| GET | `/api/v1/version` | Gets the latest database version. |

Request bodies are defined in `paskalON.Devices.Service.Dto`, response bodies in `paskalON.Devices.Dto`.


## Published Topics (Redis)
The topic names are configured in the [system configuration](./Configuration.md#system-configuration). The values below are from the demo client data.
Each device type publishes a core topic (fast changing values, every `DeviceIntervalMilliseconds` × `DeviceFactorCore`) and a detail topic (every `DeviceIntervalMilliseconds` × `DeviceFactorDetail`).

| Configuration property | Demo topic | Messages |
|------------------------|------------|----------|
| `PublisherTopicPcsCore` / `...Detail` | `ppc:device:pcs:core` / `:detail` | `PcsCoreDto` / `PcsDetailDto` |
| `PublisherTopicBatteryBankCore` / `...Detail` | `ppc:device:bb:core` / `:detail` | `BbCoreDto` / `BbDetailDto` |
| `PublisherTopicSolarPanelCore` / `...Detail` | `ppc:device:pv:core` / `:detail` | `PvCoreDto` / `PvDetailDto` |
| `PublisherTopicSystemPowerMeterCore` / `...Detail` | `ppc:device:pm:system:core` / `:detail` | `PmSystemCoreDto` / `PmSystemDetailDto` |
| `PublisherTopicCircuitPowerMeterCore` / `...Detail` | `ppc:device:pm:circuit:core` / `:detail` | `PmCircuitCoreDto` / `PmCircuitDetailDto` |
| `PublisherTopicExternalPowerMeterCore` / `...Detail` | `ppc:device:pm:external:core` / `:detail` | `PmExternalCoreDto` / `PmExternalDetailDto` |
| `PublisherTopicAuxiliaryPowerMeterCore` / `...Detail` | `ppc:device:pm:auxiliary:core` / `:detail` | `PmAuxiliaryCoreDto` / `PmAuxiliaryDetailDto` |


## Subscribed Topics (Redis)

| Configuration property | Demo topic | Publisher | Description |
|------------------------|------------|-----------|-------------|
| `SubscriberTopicPowerTarget` | `ppc:pc:targets` | Power Control | Power targets of the PCS. |


## Client Library
[paskalON.Devices.Client](../paskalON.Devices.Client/README.md) reads the DER structure over the Web API and subscribes to the device data topics for other services.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
