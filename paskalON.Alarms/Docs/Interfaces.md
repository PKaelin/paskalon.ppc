# Alarms Interfaces
APIs, messaging topics and client library exposed or consumed by the Alarms solution.

Back to the [Overview](./Overview.md).


## Web API
Base address in DEV: `http://localhost:45030`. There is no REST Client file yet.

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/v1/alarms/summary` | Gets the number of active alarms per severity level, unacknowledged and acknowledged separately (e.g. for Luna at startup). |
| GET | `/api/v1/alarms/ppcalarms` | Gets the active and normalized PPC alarms that match the filter (e.g. for the alarm list of Luna). |
| POST | `/api/v1/alarms/acknowledge` | Acknowledges the alarms that can be acknowledged. |
| POST | `/api/v1/alarmconfigs/reload` | Reloads the enabled alarm configurations from the database, e.g. after the configurations were changed. |
| GET | `/api/v1/localizations/texts` | Gets the texts of all localization keys in a language (alarm names and descriptions). |

Request and response bodies are defined in `paskalON.Alarms.Service.Dto` and `paskalON.Alarms.Dto`.


## Published Alarm Actions (RabbitMQ)

| Configuration property | Demo exchange | Message | Description |
|------------------------|---------------|---------|-------------|
| `PublisherExchangeAlarmActions` | `ppc:alarms:actions` | `AlarmActionDto` | Actions of raised, acknowledged and normalized alarms. Sent through an outbox, so no action is lost while RabbitMQ is not reachable. |


## Published Topics (Redis)

| Configuration property | Demo topic | Message | Description |
|------------------------|------------|---------|-------------|
| `PublisherTopicHeartbeat` | `ppc:alarms:heartbeat` | `HeartbeatDto` | Heartbeat of this service. |


## Subscribed Topics (Redis)
The topic names are configured in the [system configuration](./Configuration.md#system-configuration). The values below are from the demo client data.

| Configuration property | Demo topic | Publisher | Alarm type |
|------------------------|------------|-----------|------------|
| `SubscriberTopicPcsCore` / `...Detail` | `ppc:device:pcs:core` / `:detail` | Device Service | `PcsAlarm` |
| `SubscriberTopicBatteryBankCore` / `...Detail` | `ppc:device:bb:core` / `:detail` | Device Service | `BatteryAlarm` |
| `SubscriberTopicSolarPanelCore` / `...Detail` | `ppc:device:pv:core` / `:detail` | Device Service | `SolarPanelAlarm` |
| `SubscriberTopic...PowerMeterCore` / `...Detail` | `ppc:device:pm:{system,circuit,external,auxiliary}:core` / `:detail` | Device Service | `SystemPowerMeterAlarm`, `CircuitPowerMeterAlarm`, `ExternalPowerMeterAlarm`, `AuxiliaryPowerMeterAlarm` |
| `SubscriberTopicPowerTarget` | `ppc:pc:targets` | Power Control | `PowerControlAlarm` |
| `SubscriberTopicCurrentSetpoint` | `ppc:om:setpoints` | Operating Modes | `OperatingModeAlarm` |


## Client Library
[paskalON.Alarms.Client](../paskalON.Alarms.Client/README.md) calls the Web API (`AlarmServer`) and subscribes to the alarm actions for other services.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
