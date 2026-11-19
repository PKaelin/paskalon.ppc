# Operating Modes Configuration
Environment variables, secrets and the persistent configuration of the Operating Modes service.

Back to the [Overview](./Overview.md).


## Environment Variables
Set in [docker-compose.yml](../docker-compose.yml).

| Variable | Required | Description |
|----------|----------|-------------|
| `DATABASE_CONNECTION_FILE` | Yes | Secret file with the PostgreSQL connection string. |
| `MESSAGING_CONNECTION_FILE` | Yes | Secret file with the Redis connection string. |
| `DEVICESERVICE_CONNECTION_FILE` | Yes | Secret file with the Device Service base address(es), separated by `;`. |
| `DEVICE_SERVICE_ENDPOINT` | Yes | Path of the Device Service API (e.g. `/api/v1`). |
| `TELEMETRY_LOGGING_ENDPOINT` | Yes | OTLP endpoint for logs. |
| `TELEMETRY_METRICS_ENDPOINT` | Yes | OTLP endpoint for metrics. |
| `TELEMETRY_TRACING_ENDPOINT` | Yes | OTLP endpoint for traces. |
| `START_PUBLISHERS` | No (default `true`) | Starts the metrics publisher. |
| `START_WORKERS` | No (default `true`) | Starts the periodic operating mode calculation. |
| `STARTUP_RETRY_COUNT` | No | Number of retries while the database or Device Service is not reachable at startup. |
| `STARTUP_RETRY_INTERVAL_SECONDS` | No | Interval between the startup retries. |


## Secrets
The secret files for DEV are in [secrets](../secrets): `database_connection`, `messaging_connection` and `deviceservice_connections`.
How Docker secrets are used is described in [InfrastructureInternal/secrets](/InfrastructureInternal/secrets/README.md).


## System Configuration
The `SystemConfig` is stored in the database and loaded at startup. The demo values are loaded by [Client Data](/paskalON.ClientData/README.md).

| Property | Default | Description |
|----------|---------|-------------|
| `Type` | – | Power control type the system serves (e.g. BESS, Solar). |
| `ReferenceFrequency` | – | System reference frequency in Hz (50 or 60). |
| `ReferenceVoltage` | – | System reference voltage in V. |
| `NameplateMaximum/MinimumVoltage` | – | Voltage nameplate in V. |
| `NameplateMaximum/MinimumCurrent` | – | Current nameplate in A. |
| `NameplateMaximum/MinimumActivePowerWatt` | – | Active power nameplate in W. |
| `NameplateMaximum/MinimumReactivePowerVars` | – | Reactive power nameplate in var. |
| `OperatingModeCalculationIntervalMilliseconds` | 1000 | Interval between two operating mode calculations (minimum 200). |
| `MetricsIntervalMilliseconds` | 1000 | Metrics publishing interval (minimum 200). |
| `SetpointRepublishIntervalMilliseconds` | 1000 | Interval in which the current setpoints are republished even if unchanged (minimum 200). |
| `DependencyHeartbeatIntervalMilliseconds` | 500 | Interval between two heartbeats of this service (minimum 100). |
| `RedisLossTimeoutMilliseconds` | 1500 | Time Redis must be disconnected before it is lost (minimum 1000). |
| `PowerControlLossTimeoutMilliseconds` | 1500 | Time without a Power Control heartbeat before it is lost (minimum 1000). |
| `DeviceServiceLossTimeoutMilliseconds` | 3000 | Time without device data before the Device Service is lost (minimum 1000). |
| `SystemPowerMeterLossTimeoutMilliseconds` | 2000 | Time without valid system power meter data before it is lost (minimum 1000). |
| `ResumeSettleMilliseconds` | 2000 | Time all dependencies must be available after a loss before the control resumes. |
| `StartupDelay` | 5000 | Delay in ms before the metrics publishing starts, so that the operating modes are initialized. |
| `SubscriberTopic...` / `PublisherTopic...` | – | Topic names, see [Interfaces](./Interfaces.md#published-topics-redis). |

While a dependency is lost the service holds the last setpoints and resumes automatically, see [Fault Tolerance & Resilience](/Docs/03-Design/FaultTolerance.md).


## Operating Mode Configuration
- Every operating mode shall have at least one type assigned but can be assigned multiple types.
- Every operating mode has configuration data. 
- Each operating mode must include a ramp configuration, see [Ramp Model](./Design/RampModel.md).
- Each operating mode may include a curve configuration, see [Curve Configuration](./Design/CurveConfiguration.md).
- Each operating mode may include a custom configuration for future or unplanned configuration types, allowing the database schema to remain unchanged as project specific configurations are introduced.

![Operating Mode Config Overview](./Assets/Diagrams/Operating%20Mode%20Config%20Overview.drawio.svg)

The configuration classes are in [paskalON.OperatingModes.Domain.Configs](../paskalON.OperatingModes.Domain.Configs/README.md). The database schema is created with [Database Migrations](/Docs/04-Develop/DatabaseMigrations.md).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
