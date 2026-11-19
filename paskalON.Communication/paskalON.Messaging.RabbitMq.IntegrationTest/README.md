# paskalON.Messaging.RabbitMq.IntegrationTest
Integration tests of [paskalON.Messaging.RabbitMq](../paskalON.Messaging.RabbitMq/README.md) against a real RabbitMQ broker.

Part of the [Communication solution](../README.md).


## Integration Test
- Docker must be running: `TestAssembly` starts a RabbitMQ container (Testcontainers) once per test run and stops it afterwards.
- Run them with `dotnet test`; no manual broker setup is required.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
