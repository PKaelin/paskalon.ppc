# Alarms Configuration
Environment variables, secrets and the persistent configuration of the Alarms service.

Back to the [Overview](./Overview.md).


## Environment Variables
Set in [docker-compose.yml](../docker-compose.yml).

| Variable | Required | Description |
|----------|----------|-------------|
| `DATABASE_CONNECTION_FILE` | Yes | Secret file with the PostgreSQL connection string. |
| `MESSAGING_CONNECTION_FILE` | Yes | Secret file with the Redis connection string (subscribed data and heartbeat). |
| `RABBITMQ_CONNECTION_FILE` | Yes | Secret file with the RabbitMQ connection URI (alarm actions). |
| `DEVICESERVICE_CONNECTION_FILE` | Yes | Secret file with the Device Service base address. |
| `DEVICE_SERVICE_ENDPOINT` | Yes | Path of the Device Service API (e.g. `/api/v1`). |
| `TELEMETRY_LOGGING_ENDPOINT` | Yes | OTLP endpoint for logs. |
| `TELEMETRY_METRICS_ENDPOINT` | Yes | OTLP endpoint for metrics. |
| `TELEMETRY_TRACING_ENDPOINT` | Yes | OTLP endpoint for traces. |
| `START_PUBLISHERS` | No (default `true`) | Starts the alarm action outbox publisher. |
| `STARTUP_RETRY_COUNT` | No | Number of retries while the database or Device Service is not reachable at startup. |
| `STARTUP_RETRY_INTERVAL_SECONDS` | No | Interval between the startup retries. |


## Secrets
The secret files for DEV are in [secrets](../secrets): `database_connection`, `messaging_connection`, `rabbitmq_connection` and `deviceservice_connection`.
How Docker secrets are used is described in [InfrastructureInternal/secrets](/InfrastructureInternal/secrets/README.md).


## System Configuration
The `SystemConfig` is stored in the database and loaded at startup. The demo values are loaded by [Client Data](/paskalON.ClientData/README.md).

| Property | Default | Description |
|----------|---------|-------------|
| `AlarmActionSendIntervalMilliseconds` | 1000 | Interval between two sends of the pending alarm actions in the outbox. |
| `AlarmActionRetryIntervalMilliseconds` | 5000 | Time before a failed send of the alarm actions is retried. |
| `AlarmDelayCheckIntervalMilliseconds` | 500 | Interval between two checks of the delayed alarm raises and normalizations. |
| `DependencyHeartbeatIntervalMilliseconds` | 500 | Interval between two heartbeats of this service. |
| `PublisherTopicHeartbeat` | – | Redis topic of the heartbeat. |
| `PublisherExchangeAlarmActions` | – | RabbitMQ exchange of the alarm actions. |
| `SubscriberTopic...` | – | Subscribed topics, see [Interfaces](./Interfaces.md#subscribed-topics-redis). |


## Alarm Configuration
Every PPC alarm configuration (`PpcAlarmConfig`) defines:

| Property | Description |
|----------|-------------|
| `AlarmId` | Unique identifier (error code) of the alarm. |
| `Name`, `Description`, `PossibleCauses`, `Response`, `Consequences` | Localization keys of the texts shown to the operator. |
| `SeverityLevel` | Severity level of the alarm, see [Domain Model](./DomainModel.md#severity-levels). |
| `IsEnabled` | Whether the alarm is evaluated. |
| `IsAcknowledgeable` | Whether an operator can acknowledge the alarm (default true). |
| `PpcAlarmType`, `DeviceId` | Publishing device type and device. |
| `PropertyName`, `PropertyOperator`, `PropertyValue` | Condition on the published data, e.g. `GreaterThan`. |
| `TimeBeforeRaisingAlarmMilliseconds` | Time the value must stay abnormal before the alarm is raised. |
| `TimeBeforeNormalizingAlarmMilliseconds` | Time the value must stay normal before the active alarm is normalized. |
| `AlarmActions` | Actions sent on `Raise`, `Normalize` or `Acknowledge`, with key/value parameters. |

The alarm configurations and localized texts are loaded by [Client Data](/paskalON.ClientData/Docs/Design/AlarmsClientData.md). After a change, reload them with `POST /api/v1/alarmconfigs/reload`.
The configuration classes are in [paskalON.Alarms.Domain.Configs](../paskalON.Alarms.Domain.Configs/README.md). The database schema is created with [Database Migrations](/Docs/04-Develop/DatabaseMigrations.md).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
