# paskalON.Infrastructure.PostgreSql
PostgreSQL Testcontainers fixture (`PostgresFixture`) and test base (`PostgresTestBase`) for database integration tests.

Part of the [Common solution](../README.md).


## Usage
Every database integration test project implements a static `TestAssembly` class that calls `PostgresFixture.StartAsync()` in `[AssemblyInitialize]` and `PostgresFixture.StopAsync()` in `[AssemblyCleanup]`. See the [integration testing instructions](/.ai/testing/testing-integration.instructions.md#database-integration-tests).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
