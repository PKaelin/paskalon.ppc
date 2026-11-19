# Communication
Messaging and protocol libraries shared by the PPC solutions: message publisher/subscriber (Redis, RabbitMQ), heartbeats, message sequencing, dependency health and the Modbus and IEEE C37.118 protocols.

Purpose, rules and the full documentation of the library are in the [Communication Overview](./Docs/Overview.md).


## Structure

| Project | Purpose |
|---------|---------|
| [paskalON.Messaging](./paskalON.Messaging/README.md) | Message abstractions, heartbeats, sequencing and dependency health. |
| [paskalON.Messaging.Redis](./paskalON.Messaging.Redis/README.md) | Redis Pub/Sub publisher and subscriber. |
| [paskalON.Messaging.RabbitMq](./paskalON.Messaging.RabbitMq/README.md) | RabbitMQ publisher and subscriber. |
| [paskalON.Messaging.RabbitMq.Testing](./paskalON.Messaging.RabbitMq.Testing/README.md) | RabbitMQ Testcontainers fixture for integration tests. |
| [paskalON.Protocols.Modbus](./paskalON.Protocols.Modbus/README.md) | Modbus TCP client and server. |
| [paskalON.Protocols.C37118](./paskalON.Protocols.C37118/README.md) | IEEE C37.118 synchrophasor client and server. |


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
