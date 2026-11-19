# Communication Overview
Messaging and protocol libraries shared by the PPC solutions.
Communication is a library: it supports one or more solutions and is not independently deployable.

Back to the [solution README](../README.md).


## Rules
- Classes in this solution shall be thoroughly tested as they are the base of many solutions.
- Changes to classes effect every solution using these classes.
- Communication contains no business logic of a specific service.


## Libraries

| Library | Purpose |
|---------|---------|
| paskalON.Messaging | Message publisher/subscriber abstractions, heartbeats, message sequencing and dependency health monitoring. |
| paskalON.Messaging.Redis | Redis (Pub/Sub) implementation of the message publisher and subscriber. |
| paskalON.Messaging.RabbitMq | RabbitMQ publisher and subscriber (alarm actions). |
| paskalON.Messaging.RabbitMq.Testing | RabbitMQ Testcontainers fixture for integration tests. |
| paskalON.Protocols.Modbus | Modbus TCP client and server (NModbus). |
| paskalON.Protocols.C37118 | IEEE C37.118 synchrophasor client and server. |


## Documentation
- [Dependencies](./Dependencies.md): which solutions use which library and the external packages.
- [Interfaces](./Interfaces.md): the main public types of each library.
- [Domain Model](./DomainModel.md): messages, heartbeats and dependency states.
- [Configuration](./Configuration.md): configuration of the libraries.
- [Design](./Design/Overview.md): message sequencing, dependency health and the C37.118 protocol.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
