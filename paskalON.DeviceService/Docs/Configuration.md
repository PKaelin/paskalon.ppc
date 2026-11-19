# Device Service Configuration
Environment variables, secrets and the persistent configuration of the Device Service.

Back to the [Overview](./Overview.md).


## Environment Variables
Set in [docker-compose.yml](../docker-compose.yml).

| Variable | Required | Description |
|----------|----------|-------------|
| `DATABASE_CONNECTION_FILE` | Yes | Secret file with the PostgreSQL connection string. |
| `MESSAGING_CONNECTION_FILE` | Yes | Secret file with the Redis connection string. |
| `TELEMETRY_LOGGING_ENDPOINT` | Yes | OTLP endpoint for logs. |
| `TELEMETRY_METRICS_ENDPOINT` | Yes | OTLP endpoint for metrics. |
| `TELEMETRY_TRACING_ENDPOINT` | Yes | OTLP endpoint for traces. |
| `START_PUBLISHERS` | No (default `true`) | Starts the device data and metrics publishers. |
| `START_WORKERS` | No (default `true`) | Starts the Modbus polling, C37 streaming and device heartbeat workers. |


## Secrets
The secret files for DEV are in [secrets](../secrets): `database_connection` and `messaging_connection`.
How Docker secrets are used is described in [InfrastructureInternal/secrets](/InfrastructureInternal/secrets/README.md).


## System Configuration
The `SystemConfig` is stored in the database and loaded at startup. The demo values are loaded by [Client Data](/paskalON.ClientData/README.md).

| Property | Default | Description |
|----------|---------|-------------|
| `Type` | – | Device service type (e.g. BESS, Solar). |
| `MetricsIntervalMilliseconds` | 1000 | Metrics publishing interval. |
| `PollingIntervalMilliseconds` | 1000 | Modbus polling interval. |
| `ModbusReconnectIntervalMilliseconds` | 30000 | Interval in which lost Modbus connections are reconnected. |
| `C37ReconnectIntervalMilliseconds` | 30000 | Interval in which lost C37 connections are reconnected. |
| `DeviceIntervalMilliseconds` | 1000 | Device publishing interval. |
| `DeviceHeartbeatIntervalMilliseconds` | 1000 | Device heartbeat interval. |
| `DeviceFactorCore` | 1 | Device publishing factor for core data (multiplied with the device interval). |
| `DeviceFactorDetail` | 5 | Device publishing factor for detail data (multiplied with the device interval). |
| `StartupDelayForDevices` | 5000 | Startup delay in ms so that the devices have some time to connect. |
| `SubscriberTopic...` / `PublisherTopic...` | – | Topic names, see [Interfaces](./Interfaces.md#published-topics-redis). |


## Device Configuration
We define a specific "Devices" just once (PCS, Battery, etc.) and have multiple "Communications" (C37 or Modbus Addressable), communicate with actual device. "Logicals" are groups of things like DerCircuit, DerUnit, etc.

![DeviceService Domain Configs Concept](./Assets/Diagrams/DeviceService%20Domain%20Configs%20Concept.drawio.svg)


### Device Domain Config Design
![DeviceService Domain Configs](./Assets/Diagrams/DeviceService%20Domain%20Configs.drawio.svg)


### Device Domain Meter Config Design
![DeviceService Domain Meter Configs](./Assets/Diagrams/DeviceService%20Domain%20Meter%20Configs.drawio.svg)


### Device Domain GMD Config Design
![DeviceService Domain GMD Configs](./Assets/Diagrams/DeviceService%20Domain%20GMD%20Configs.drawio.svg)

The configuration classes are in [paskalON.Devices.Domain.Configs](../paskalON.Devices.Domain.Configs/README.md). The database schema is created with [Database Migrations](/Docs/04-Develop/DatabaseMigrations.md).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
