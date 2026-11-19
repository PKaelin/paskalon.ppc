# Alarms Overview
The Alarms service contains all logic regarding the evaluation and lifecycle of PPC alarms.
It subscribes to the data published by the other services, evaluates the data against the alarm configurations, persists raised/normalized alarms and publishes the alarm actions.

Back to the [solution README](../README.md).


## Responsibilities
- Load the enabled alarm configurations, alarm actions and localized texts from its database.
- Evaluate the published device data, power targets and setpoints against the alarm configurations.
- Raise and normalize alarms after their configured delays and persist them.
- Publish the alarm actions reliably through an outbox to RabbitMQ.
- Provide the active alarms, the alarm summary, acknowledgement and localized texts over the Web API.


## Boundaries
- The Alarms service only reads the published data of the other services. It does not command devices or change setpoints.
- Operators see and acknowledge alarms in [Luna](/paskalON.Luna/README.md), which uses [paskalON.Alarms.Client](../paskalON.Alarms.Client/README.md).


## Documentation
- [Dependencies](./Dependencies.md): services, infrastructure and libraries the solution depends on.
- [Interfaces](./Interfaces.md): Web API, subscribed topics, published alarm actions and client library.
- [Domain Model](./DomainModel.md): alarms, severities and the alarm sequence.
- [Configuration](./Configuration.md): environment variables, secrets, system and alarm configuration.
- [Design](./Design/Overview.md): alarm lifecycle.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
