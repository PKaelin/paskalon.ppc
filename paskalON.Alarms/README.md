# Alarms
This solution is to self contain all logic regarding the evaluation and lifecycle of PPC alarms.
The alarm service subscribes to the data published by the other services, evaluates the data against the alarm configurations,
persists raised/normalized alarms and publishes the alarms and their alarm actions.

Purpose, responsibilities and the full documentation of the solution are in the [Alarms Overview](./Docs/Overview.md).


## Structure

| Project | Purpose |
|---------|---------|
| [paskalON.Alarms.Domain.Configs](./paskalON.Alarms.Domain.Configs/README.md) | Persistent domain configuration layer of DDD (alarm, alarm action and system configurations). |
| [paskalON.Alarms.Domain](./paskalON.Alarms.Domain/README.md) | Domain layer of DDD (alarms, alarm sequence states, evaluators). |
| [paskalON.Alarms.Dto](./paskalON.Alarms.Dto/README.md) | Data transfer objects published by the alarm service (alarms and alarm actions). |
| [paskalON.Alarms.Infrastructure](./paskalON.Alarms.Infrastructure/README.md) | Infrastructure layer of DDD (database context, repositories, migrations). |
| [paskalON.Alarms.Application](./paskalON.Alarms.Application/README.md) | Application layer of DDD (handlers, value store, evaluator orchestration, outbox, queries). |
| [paskalON.Alarms.Client](./paskalON.Alarms.Client/README.md) | Client library to call the alarm service API and to subscribe to the alarm actions. |
| [paskalON.Alarms.Service.Dto](./paskalON.Alarms.Service.Dto/README.md) | Request/response objects of the alarm service Web API. |
| [paskalON.Alarms.Service](./paskalON.Alarms.Service/README.md) | Service/interface layer (Web API, hosted workers, dependency injection). |

*Note: paskalON.Common and paskalON.Communication libraries are project references during initial development.*


## Docker Compose
[docker-compose.yml](./docker-compose.yml) builds and starts the Alarms service container (`paskalon.alarms.service`, port 45030) in the network `ppc.network`.
The database, Redis, RabbitMQ and telemetry run in [InfrastructureInternal](/InfrastructureInternal/README.md), which must be started first.


## Web API
The Web API provides the active alarms, the alarm summary, acknowledgement, the localized texts and reloads the alarm configurations. The endpoints are described in [Interfaces](./Docs/Interfaces.md#web-api).
Requests are in the REST Client file [paskalON.Alarms.Service.http](./paskalON.Alarms.Service/paskalON.Alarms.Service.http).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
