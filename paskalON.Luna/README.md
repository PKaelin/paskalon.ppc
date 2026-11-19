# Luna (User Interface)
Luna is the operator user interface of the PPC: the Luna Web single page application and the Luna Web API that connects it to the backend services.

Purpose, responsibilities and the full documentation of the solution are in the [Luna Overview](./Docs/Overview.md).


## Structure

| Project | Purpose |
|---------|---------|
| [paskalON.Luna.Domain.Configs](./paskalON.Luna.Domain.Configs/README.md) | System configuration of the Luna Web API. |
| [paskalON.Luna.Application](./paskalON.Luna.Application/README.md) | Application layer: managers that call the backend services. |
| [paskalON.Luna.Dto](./paskalON.Luna.Dto/README.md) | DTOs owned by Luna. |
| [paskalON.Luna.Service](./paskalON.Luna.Service/README.md) | Luna Web API: REST controllers, SignalR hubs and publishers. |
| [paskalON.Luna.Web](./paskalON.Luna.Web/README.md) | Luna Web single page application (Vue 3). |

*Note: paskalON.Common and paskalON.Communication libraries are project references during initial development.*


## Docker Compose
[docker-compose.yml](./docker-compose.yml) builds and starts the Luna Web API container (`paskalon.luna.service`, port 45550) and the Luna Web container (`paskalon.luna.web`, port 45552) in the network `ppc.network`.
Redis, RabbitMQ, telemetry and Grafana run in [InfrastructureInternal](/InfrastructureInternal/README.md), which must be started first. The backend services must be running to see data.


## Web API
The Luna Web API forwards the operator commands to the backend services and pushes live values with SignalR. The endpoints and hubs are described in [Interfaces](./Docs/Interfaces.md).
REST Client file: [paskalON.Luna.Service.http](./paskalON.Luna.Service/paskalON.Luna.Service.http).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
