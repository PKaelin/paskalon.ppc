# Luna Interfaces
APIs, SignalR hubs and messaging topics exposed or consumed by the Luna solution.

Back to the [Overview](./Overview.md).


## Web API
Base address in DEV: `http://localhost:45550`. Requests are in the REST Client file [paskalON.Luna.Service.http](../paskalON.Luna.Service/paskalON.Luna.Service.http).
Luna Web reaches the Web API through its nginx proxy under `/api`.


### Device (`/api/v1/device/...`)

| Method | Action | Description |
|--------|--------|-------------|
| GET | `getder` | Gets the DER root with all its contents. |
| POST | `setmaintenance` | Puts a DER unit into or out of maintenance. |
| POST | `startallpcs` / `startpcs` | Starts all PCS that are not in maintenance mode / a specific PCS. |
| POST | `stopallpcs` / `stoppcs` | Stops all PCS that are not in maintenance mode / a specific PCS. |
| POST | `standbyallpcs` / `standbypcs` | Puts all PCS that are not in maintenance mode / a specific PCS into standby. |
| POST | `connectbatterybank` / `disconnectbatterybank` | Connects / disconnects a specific battery bank. |


### Operating Mode (`/api/v1/operatingmode/...`)

| Method | Action | Description |
|--------|--------|-------------|
| GET | `getavailableoperatingmodes` | Gets all stackable operating modes that can be added to the stack. |
| GET | `getactiveoperatingmodes` | Gets the operating modes of the stack in ascending priority order. |
| POST | `addoperatingmode` | Adds an operating mode to the stack with the specified name and priority. |
| POST | `moveoperatingmode` | Moves an operating mode of the stack to a new priority. |
| POST | `removeoperatingmode` | Removes an operating mode from the stack. |
| POST | `replaceoperatingmode` | Replaces an operating mode of the stack in one atomic change. |
| POST | `setoperatingmodes` | Sets the whole stack in one atomic change. |
| GET | `getsetpoint` | Gets the supervisory setpoints of the system. |
| POST | `setsetpoint` | Sets the supervisory setpoints of the system. |
| POST | `activateoverrideoperatingmode` | Activates an override operating mode, e.g. a shutdown or the maintenance of a DER unit. |
| POST | `deactivateoverrideoperatingmode` | Deactivates an override operating mode. |


### System (`/api/v1/system/...`)

| Method | Action | Description |
|--------|--------|-------------|
| GET | `getnameplate` | Gets the system nameplate ratings (minimum and maximum active and reactive power). |
| POST | `systemstartcommand` | Starts all PCS. Rejected with 409 Conflict while a shutdown is active. |
| POST | `systemstopcommand` | Shuts the system down by activating the shutdown override operating mode. |
| POST | `clearshutdowncommand` | Clears the shutdown by deactivating the shutdown override operating mode. |


### Alarm (`/api/v1/alarm/...`)

| Method | Action | Description |
|--------|--------|-------------|
| GET | `getppcalarms` | Gets the PPC alarms triggered within the last hours that match the other criteria. |
| GET | `getlocalizedtexts` | Gets the texts of the localization keys of the alarm configurations in the language of the user. |
| POST | `acknowledgealarms` | Acknowledges the alarms in the alarm service. |


## SignalR Hubs

| Hub | Route | Description |
|-----|-------|-------------|
| `SystemHub` | `/hubs/system` | Live system values (system meter summary and setpoints). |
| `DeviceHub` | `/hubs/device` | Live values of the subscribed device, DER unit, circuit or group. |
| `AlarmHub` | `/hubs/alarm` | Alarm summary for the alarm shapes in the header. |
| `DependencyHub` | `/hubs/dependency` | Availability of the dependencies. |

The publishing rules (publish on demand, initial value, connection status) are described in [Real-Time Data](./Design/RealTimeData.md).


## Subscribed Topics
The topic names are configured in [SystemConfig.xml](./Configuration.md#system-configuration).

| Configuration property | Default | Publisher | Description |
|------------------------|---------|-----------|-------------|
| `SubscriberTopicCurrentSetpoint` | `ppc:om:setpoints` | Operating Modes | System setpoints. |
| `SubscriberTopicCurrentDerUnitSetpoint` | `ppc:om:setpoints:unit` | Operating Modes | DER unit setpoints. |
| `SubscriberTopicPowerTarget` | `ppc:pc:targets` | Power Control | Power targets. |
| `SubscriberTopicPowerControlHeartbeat` | `ppc:pc:heartbeat` | Power Control | Dependency status. |
| `SubscriberTopicOperatingModeHeartbeat` | `ppc:om:heartbeat` | Operating Modes | Dependency status. |
| `SubscriberTopicAlarmServiceHeartbeat` | `ppc:alarms:heartbeat` | Alarms | Dependency status. |
| `SubscriberTopic...Core` / `...Detail` | `ppc:device:...` | Device Service | Device data (PCS, battery bank, solar, power meters). |
| `SubscriberExchangeAlarmActions` (RabbitMQ) | `ppc:alarms:actions` | Alarms | Alarm actions that update the alarm summary. |


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
