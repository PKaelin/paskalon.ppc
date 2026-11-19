# Communication Dependencies
Which solutions use the Communication libraries and which packages and libraries they depend on.

Back to the [Overview](./Overview.md).


## Used By

| Library | Used by |
|---------|---------|
| paskalON.Messaging | Device Service, Power Control, Operating Modes, Alarms, Luna |
| paskalON.Messaging.Redis | Device Service, Power Control, Operating Modes, Alarms, Luna |
| paskalON.Messaging.RabbitMq | Alarms, Luna (via paskalON.Alarms.Client) |
| paskalON.Messaging.RabbitMq.Testing | paskalON.Messaging.RabbitMq.IntegrationTest |
| paskalON.Protocols.Modbus | Device Service, Device Simulator |
| paskalON.Protocols.C37118 | Device Service, Device Simulator |


## Packages And Libraries

| Library | Packages | Libraries |
|---------|----------|-----------|
| paskalON.Messaging | Microsoft.Extensions.Hosting.Abstractions, Microsoft.Extensions.Logging.Abstractions | – |
| paskalON.Messaging.Redis | StackExchange.Redis | paskalON.Messaging |
| paskalON.Messaging.RabbitMq | RabbitMQ.Client, Microsoft.Extensions.Logging.Abstractions | – |
| paskalON.Messaging.RabbitMq.Testing | Testcontainers.RabbitMq | paskalON.Messaging.RabbitMq |
| paskalON.Protocols.Modbus | NModbus, Microsoft.Extensions.Logging.Abstractions | paskalON.Dataface ([Common](/paskalON.Common/README.md)) |
| paskalON.Protocols.C37118 | Microsoft.Extensions.Logging.Abstractions | paskalON.Dataface ([Common](/paskalON.Common/README.md)) |


## Infrastructure
Redis and RabbitMQ are provided by [InfrastructureInternal](/InfrastructureInternal/README.md) in DEV and TST.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
