# Real-Time Data
How the Luna Web API pushes live values to Luna Web with SignalR.

Back to the [Design Overview](./Overview.md).


## Rules
The rules are defined in the [user interface instructions](/.ai/core/user-interface.instructions.md#real-time-data-signalr). In short:
- **Publish on demand:** data is published only while the page or component that needs it is shown. Header data (alarm summary, dependency status) is published to all connected clients.
- **Server subscriptions:** a hub carries one topic or tracks per connection which device is subscribed (`DeviceSubscriptions`). Publishers skip the work when no connection is subscribed.
- **Initial value:** on connect or subscribe the hub sends the current value immediately.
- **Connection status:** Luna Web shows the hub connection status next to the data and a warning hint while the connection is lost.


## Hubs

| Hub | Client methods | Subscription |
|-----|----------------|--------------|
| `SystemHub` | `SystemMeterSummaryUpdated`, `SetpointsUpdated` | All connections of the hub. |
| `DeviceHub` | `PcsUpdated`, `BatteryBankUpdated`, `SystemPowerMeterUpdated`, `ExternalPowerMeterUpdated`, `AuxiliaryPowerMeterUpdated`, `BatteryStorageUnitUpdated`, `DerCircuitUpdated`, `DerGroupUpdated` | Per connection and device (`DeviceSubscriptions`). |
| `AlarmHub` | `AlarmSummaryUpdated` | All connections (header). |
| `DependencyHub` | `DependencyStatusUpdated` | All connections (header). |

The routes are listed in [Interfaces](../Interfaces.md#signalr-hubs).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
