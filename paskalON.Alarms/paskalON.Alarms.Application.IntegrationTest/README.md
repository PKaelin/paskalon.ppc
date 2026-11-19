# paskalON.Alarms.Application.IntegrationTest
Integration tests of [paskalON.Alarms.Application](../paskalON.Alarms.Application/README.md) with the real infrastructure and a PostgreSQL database (alarm handler and outbox).

Part of the [Alarms solution](../README.md).


## Integration Test
- Docker must be running: `TestAssembly` starts a PostgreSQL container (Testcontainers) once per test run and stops it afterwards.
- `DatabaseTestBase` creates a temporary database for each test, so the tests do not depend on each other.
- Run them with `dotnet test`; no manual database setup is required.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
