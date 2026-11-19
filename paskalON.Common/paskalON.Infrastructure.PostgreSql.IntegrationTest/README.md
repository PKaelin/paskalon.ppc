# paskalON.Infrastructure.PostgreSql.IntegrationTest
Integration tests of [paskalON.Infrastructure.PostgreSql](../paskalON.Infrastructure.PostgreSql/README.md) against a real PostgreSQL database (fixture, test base and sample migrations).

Part of the [Common solution](../README.md).


## Integration Test
- Docker must be running: `TestAssembly` starts a PostgreSQL container (Testcontainers) once per test run and stops it afterwards.
- Run them with `dotnet test`; no manual database setup is required.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
