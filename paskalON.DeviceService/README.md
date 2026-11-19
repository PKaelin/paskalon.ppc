# Device Service
This solution is to self contain all logic regarding the implementation of a Device Service.
The service acts as a translator between a power controller and the physical devices (inverters, battery banks, etc.)

Purpose, responsibilities and the full documentation of the solution are in the [Device Service Overview](./Docs/Overview.md).


## Structure

| Project | Purpose |
|---------|---------|
| [paskalON.Devices.Domain.Configs](./paskalON.Devices.Domain.Configs/README.md) | Persistent domain configuration layer of DDD. |
| [paskalON.Devices.Domain](./paskalON.Devices.Domain/README.md) | Domain layer of DDD: DER structure and devices. |
| [paskalON.Devices.Equipments](./paskalON.Devices.Equipments/README.md) | Equipment and manufacturer specific implementations. |
| [paskalON.Devices.Infrastructure](./paskalON.Devices.Infrastructure/README.md) | Infrastructure layer of DDD: database context, repositories and migrations. |
| [paskalON.Devices.Application](./paskalON.Devices.Application/README.md) | Application layer of DDD: device creation, data publishing and power target handling. |
| [paskalON.Devices.Dto](./paskalON.Devices.Dto/README.md) | Device data and DER structure DTOs shared with other services. |
| [paskalON.Devices.Client](./paskalON.Devices.Client/README.md) | DER structure and device data client for other services. |
| [paskalON.Devices.Service.Dto](./paskalON.Devices.Service.Dto/README.md) | Request DTOs of the Web API. |
| [paskalON.Devices.Service](./paskalON.Devices.Service/README.md) | Service/interface layer: Web API, device communication workers and publishers. |

*Note: paskalON.Common and paskalON.Communication libraries are project references during initial development.*


## Docker Compose
[docker-compose.yml](./docker-compose.yml) builds and starts the Device Service container (`paskalon.devices.service`, port 45000) in the network `ppc.network`.
The database, Redis and telemetry run in [InfrastructureInternal](/InfrastructureInternal/README.md), which must be started first.


## Web API
The Web API provides the DER structure and sends commands to the devices (start, stop, standby, power target, connect, maintenance). The endpoints are described in [Interfaces](./Docs/Interfaces.md#web-api).
REST Client file: [paskalON.Devices.Service.http](./paskalON.Devices.Service/paskalON.Devices.Service.http).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
