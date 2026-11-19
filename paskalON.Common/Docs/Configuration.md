# Common Configuration
The Common libraries have no configuration of their own. They are configured by the solutions that use them.

Back to the [Overview](./Overview.md).


## Configured By The Using Solutions
- `StartupRetryOptions` (paskalON.Hosting): `STARTUP_RETRY_COUNT` and `STARTUP_RETRY_INTERVAL_SECONDS` environment variables of the services.
- `MetricsPublisher` (paskalON.Telemetry): publishing interval from the system configuration of the services.
- `PostgresFixture` (paskalON.Infrastructure.PostgreSql): started by the `TestAssembly` class of each database integration test project.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
