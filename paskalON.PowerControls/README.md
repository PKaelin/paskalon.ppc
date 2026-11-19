# Power Control
This solution is to self contain all logic regarding the power control and its constraint engine.
It receives the setpoints of the operating modes, applies the constraints, distributes the power between the DER units and publishes the power targets to the device service.

Purpose, responsibilities and the full documentation of the solution are in the [Power Control Overview](./Docs/Overview.md).


## Structure

| Project | Purpose |
|---------|---------|
| [paskalON.ConstraintEngine.Domain.Configs](./paskalON.ConstraintEngine.Domain.Configs/README.md) | Persistent domain configuration layer of the constraint engine. |
| [paskalON.ConstraintEngine.Domain](./paskalON.ConstraintEngine.Domain/README.md) | Domain layer of the constraint engine: power and ramp constraints. |
| [paskalON.PowerControls.Domain.Configs](./paskalON.PowerControls.Domain.Configs/README.md) | Persistent domain configuration layer of DDD. |
| [paskalON.PowerControls.Domain](./paskalON.PowerControls.Domain/README.md) | Domain layer of DDD: power controls, maps and distribution strategies. |
| [paskalON.PowerControls.Infrastructure](./paskalON.PowerControls.Infrastructure/README.md) | Infrastructure layer of DDD: database context, repositories and migrations. |
| [paskalON.PowerControls.Application](./paskalON.PowerControls.Application/README.md) | Application layer of DDD: power control manager, subscribers and publishers. |
| [paskalON.PowerControls.Dto](./paskalON.PowerControls.Dto/README.md) | Messages shared with other services. |
| [paskalON.PowerControls.Client](./paskalON.PowerControls.Client/README.md) | Power target subscriber for other services. |
| [paskalON.PowerControls.Service.Dto](./paskalON.PowerControls.Service.Dto/README.md) | Request DTOs of the Web API. |
| [paskalON.PowerControls.Service](./paskalON.PowerControls.Service/README.md) | Service/interface layer: Web API, heartbeat and metrics publisher. |

*Note: paskalON.Common and paskalON.Communication libraries are project references during initial development.*


## Docker Compose
[docker-compose.yml](./docker-compose.yml) builds and starts the Power Control service container (`paskalon.powercontrols.service`, port 45010) in the network `ppc.network`.
The database, Redis and telemetry run in [InfrastructureInternal](/InfrastructureInternal/README.md), which must be started first.


## Web API
The Web API sets system and DER unit power setpoints directly and starts, stops or puts all PCS in standby. The endpoints are described in [Interfaces](./Docs/Interfaces.md#web-api).
REST Client file: [paskalON.PowerControls.Service.http](./paskalON.PowerControls.Service/paskalON.PowerControls.Service.http).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
