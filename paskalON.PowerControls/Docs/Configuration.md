# Power Control Configuration
Environment variables, secrets and the persistent configuration of the Power Control service.

Back to the [Overview](./Overview.md).


## Environment Variables
Set in [docker-compose.yml](../docker-compose.yml).

| Variable | Required | Description |
|----------|----------|-------------|
| `DATABASE_CONNECTION_FILE` | Yes | Secret file with the PostgreSQL connection string. |
| `MESSAGING_CONNECTION_FILE` | Yes | Secret file with the Redis connection string. |
| `DEVICESERVICE_CONNECTION_FILE` | Yes | Secret file with the Device Service base address. |
| `DEVICE_SERVICE_ENDPOINT` | Yes | Path of the Device Service API (e.g. `/api/v1`). |
| `TELEMETRY_LOGGING_ENDPOINT` | Yes | OTLP endpoint for logs. |
| `TELEMETRY_METRICS_ENDPOINT` | Yes | OTLP endpoint for metrics. |
| `TELEMETRY_TRACING_ENDPOINT` | Yes | OTLP endpoint for traces. |
| `START_PUBLISHERS` | No (default `true`) | Starts the metrics publisher. |
| `STARTUP_RETRY_COUNT` | No | Number of retries while the database or Device Service is not reachable at startup. |
| `STARTUP_RETRY_INTERVAL_SECONDS` | No | Interval between the startup retries. |


## Secrets
The secret files for DEV are in [secrets](../secrets): `database_connection`, `messaging_connection` and `deviceservice_connection`.
How Docker secrets are used is described in [InfrastructureInternal/secrets](/InfrastructureInternal/secrets/README.md).


## System Configuration
The `SystemConfig` is stored in the database and loaded at startup. The demo values are loaded by [Client Data](/paskalON.ClientData/README.md).

| Property | Default | Description |
|----------|---------|-------------|
| `Type` | – | Power control type (e.g. BESS, Solar). |
| `MetricsIntervalMilliseconds` | 1000 | Metrics publishing interval. |
| `DependencyHeartbeatIntervalMilliseconds` | 500 | Interval between two heartbeats of this service. |
| `StartupDelayForDevices` | 5000 | Startup delay in ms so that the power control is properly initialized. |
| `SubscriberTopic...` / `PublisherTopic...` | – | Topic names, see [Interfaces](./Interfaces.md#published-topics-redis). |


## Power Control Configuration
At this point we only have two major power controls:
- System power controls
- Der Unit power controls

![PowerControl Diagram](./Assets/Diagrams/PowerControlConfigs%20Diagram.drawio.svg)


## Constraint Configuration
At this point we only have two major constraints:
- Power constraints
- Ramp constraints

Those two constraints are for the System or for DerUnit's.

![ConstraintEngine Diagram](./Assets/Diagrams/ConstraintConfigs%20Diagram.drawio.svg)

The configuration classes are in [paskalON.PowerControls.Domain.Configs](../paskalON.PowerControls.Domain.Configs/README.md) and [paskalON.ConstraintEngine.Domain.Configs](../paskalON.ConstraintEngine.Domain.Configs/README.md). The database schema is created with [Database Migrations](/Docs/04-Develop/DatabaseMigrations.md).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
