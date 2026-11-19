# paskalON.Messaging.RabbitMq.Testing
RabbitMQ Testcontainers fixture (`RabbitMqFixture`) and test recipient for integration tests.

Part of the [Communication solution](../README.md).


## Usage
The integration test project implements a static `TestAssembly` class that calls `RabbitMqFixture.StartAsync()` in `[AssemblyInitialize]` and `RabbitMqFixture.StopAsync()` in `[AssemblyCleanup]`.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
