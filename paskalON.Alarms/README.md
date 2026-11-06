# Alarms
This solution is to self contain all logic regarding the evaluation and lifecycle of PPC alarms.
The alarm service subscribes to the data published by the other services, evaluates the data against the alarm configurations,
persists raised/normalized alarms and publishes the alarms and their alarm actions.  

*Note: paskalON.Common libraries are project references during initial development*  


## Structure

### Project .Domain.Configs
Persistent Domain Configuration layer of DDD (alarm configurations, alarm action configurations, system configuration).

### Project .Domain
Domain layer of DDD (alarms, alarm sequence states, alarm process conditions, evaluators).

### Project .Dto
Data transfer objects published by the alarm service (alarms and alarm actions).

### Project .Infrastructure
Infrastructure layer of DDD (database context, repositories, migrations).

### Project .Application
Application layer of DDD (event handlers, stateless store, evaluator orchestration, persister, publishers).

### Project .Client
Client library for other services to call the alarm service API and to subscribe to the alarm publications.

### Project .Service.Dto
Request/response objects of the alarm service Web API.

### Project .Service
Service/Interface layer (Web API, hosted workers, dependency injection).
