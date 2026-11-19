# Luna Domain Model
Luna has no domain model of its own. The domains are owned by the backend services, Luna maps their DTOs for the user interface.

Back to the [Overview](./Overview.md).


## Application Managers
`paskalON.Luna.Application` contains one manager per area:

| Manager | Purpose |
|---------|---------|
| `LunaDeviceManager` | DER structure, device values and device commands. |
| `LunaOperatingModeManager` | Operating mode stack, overrides, setpoints and setpoint status. |
| `LunaPowerControlManager` | Power targets. |
| `LunaSystemManager` | System nameplate and system commands (start, stop/shutdown, clear shutdown). |
| `LunaAlarmManager` | Alarm list, alarm summary, acknowledgement and localized texts. |
| `LunaDependencyManager` | Availability of the dependencies. |


## DTOs
`paskalON.Luna.Dto` contains the DTOs owned by Luna: system meter summary, dependency status, system nameplate, system command response and alarm acknowledgement.
The TypeScript types in `paskalON.Luna.Web/src/types` mirror these DTOs.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
