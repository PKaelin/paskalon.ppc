# Luna Overview
Luna is the operator user interface of the PPC. It consists of the Luna Web single page application and the Luna Web API (`paskalON.Luna.Service`).
All requests of Luna Web go through the Luna Web API, which forwards them to the backend services and pushes live values to the browser with SignalR.

Back to the [solution README](../README.md).


## Responsibilities
- Show the system values, the devices, the operating modes and the alarms to the operator.
- Forward operator commands (operating mode stack, setpoints, overrides, device commands, alarm acknowledgement) to the backend services.
- Push live values (system, device, alarm summary, dependency status) to the browser while the page that needs them is shown.
- Monitor the availability of its dependencies and show it in the header.
- Embed the Grafana dashboards.


## Boundaries
- Luna has no database and no business logic of its own. It calls the Web APIs of the [Device Service](/paskalON.DeviceService/README.md), [Operating Modes](/paskalON.OperatingModes/README.md) and [Alarms](/paskalON.Alarms/README.md) services and subscribes to their published data.
- Luna Web never calls the backend services or databases directly. Grafana embedding is the only exception.
- The user interface rules are defined in [user-interface.instructions.md](/.ai/core/user-interface.instructions.md).


## Documentation
- [Dependencies](./Dependencies.md): services, infrastructure and libraries the solution depends on.
- [Interfaces](./Interfaces.md): Web API, SignalR hubs and subscribed topics.
- [Domain Model](./DomainModel.md): application managers and DTOs.
- [Configuration](./Configuration.md): environment variables, secrets, system and web configuration.
- [Design](./Design/Overview.md): header events and real-time data.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
