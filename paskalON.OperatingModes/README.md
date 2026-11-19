# Operating Modes
This solution is to self contain all logic regarding the implementation of Operating Modes.
An Operating Mode defines the specific behavior and control strategy the system uses to interact with the power grid.

Purpose, responsibilities and the full documentation of the solution are in the [Operating Modes Overview](./Docs/Overview.md).


## Structure

| Project | Purpose |
|---------|---------|
| [paskalON.OperatingModes.Domain.Configs](./paskalON.OperatingModes.Domain.Configs/README.md) | Persistent domain configuration layer of DDD. |
| [paskalON.OperatingModes.Domain](./paskalON.OperatingModes.Domain/README.md) | Domain layer of DDD: operating modes, stack, transitions, ramps and curves. |
| [paskalON.OperatingModes.Infrastructure](./paskalON.OperatingModes.Infrastructure/README.md) | Infrastructure layer of DDD: database context, repositories and migrations. |
| [paskalON.OperatingModes.Application](./paskalON.OperatingModes.Application/README.md) | Application layer of DDD: factories, stack and override management, setpoint publishing. |
| [paskalON.OperatingModes.Dto](./paskalON.OperatingModes.Dto/README.md) | Messages and response DTOs shared with other services. |
| [paskalON.OperatingModes.Client](./paskalON.OperatingModes.Client/README.md) | Setpoint subscriber for other services. |
| [paskalON.OperatingModes.Service.Dto](./paskalON.OperatingModes.Service.Dto/README.md) | Request DTOs of the Web API. |
| [paskalON.OperatingModes.Service](./paskalON.OperatingModes.Service/README.md) | Service/interface layer: Web API, calculation worker and metrics publisher. |

*Note: paskalON.Common and paskalON.Communication libraries are project references during initial development.*


## Docker Compose
[docker-compose.yml](./docker-compose.yml) builds and starts the Operating Modes service container (`paskalon.operatingmodes.service`, port 45020) in the network `ppc.network`.
The database, Redis and telemetry run in [InfrastructureInternal](/InfrastructureInternal/README.md), which must be started first.


## Web API
The Web API manages the operating mode stack, the overrides and the operator setpoints and starts, stops or puts all PCS in standby. The endpoints are described in [Interfaces](./Docs/Interfaces.md#web-api).
REST Client file: [paskalON.OperatingModes.Service.http](./paskalON.OperatingModes.Service/paskalON.OperatingModes.Service.http).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
