# Luna Configuration
Environment variables, secrets and the configuration of the Luna Web API and Luna Web.

Back to the [Overview](./Overview.md).


## Luna Web API Environment Variables
Set in [docker-compose.yml](../docker-compose.yml).

| Variable | Required | Description |
|----------|----------|-------------|
| `MESSAGING_CONNECTION_FILE` | Yes | Secret file with the Redis connection string. |
| `RABBITMQ_CONNECTION_FILE` | Yes | Secret file with the RabbitMQ connection URI (alarm actions). |
| `DEVICESERVICE_CONNECTION_FILE` / `DEVICE_SERVICE_ENDPOINT` | Yes | Secret file with the Device Service base address and the API path (e.g. `/api/v1`). |
| `OPERATINGMODE_CONNECTION_FILE` / `OPERATINGMODE_SERVICE_ENDPOINT` | Yes | Secret file with the Operating Modes base address and the API path. |
| `ALARMSERVICE_CONNECTION_FILE` / `ALARM_SERVICE_ENDPOINT` | Yes | Secret file with the Alarms base address and the API path. |
| `TELEMETRY_LOGGING_ENDPOINT` | Yes | OTLP endpoint for logs. |
| `TELEMETRY_METRICS_ENDPOINT` | Yes | OTLP endpoint for metrics. |
| `TELEMETRY_TRACING_ENDPOINT` | Yes | OTLP endpoint for traces. |
| `STARTUP_RETRY_COUNT` | No | Number of retries while a dependency is not reachable at startup. |
| `STARTUP_RETRY_INTERVAL_SECONDS` | No | Interval between the startup retries. |


## Secrets
The secret files for DEV are in [secrets](../secrets): `messaging_connection`, `rabbitmq_connection`, `deviceservice_connection`, `operatingmode_connection` and `alarmservice_connection`.
How Docker secrets are used is described in [InfrastructureInternal/secrets](/InfrastructureInternal/secrets/README.md).


## System Configuration
Luna has no database. The system configuration is read from [SystemConfig.xml](../paskalON.Luna.Service/SystemConfig.xml) at startup.

| Property | Description |
|----------|-------------|
| `SubscriberTopic...` / `SubscriberExchangeAlarmActions` | Subscribed topics and exchange, see [Interfaces](./Interfaces.md#subscribed-topics). |
| `ShutdownOperatingModeName` | Name of the shutdown override operating mode configured in the operating mode service. |
| `Maximum/MinimumActivePowerSystemNameplateWatts` | System nameplate rating for active power in W. |
| `Maximum/MinimumReactivePowerSystemNameplateVars` | System nameplate rating for reactive power in var. |
| `RedisLossTimeoutMilliseconds` | Time Redis must be disconnected before it is lost. |
| `DeviceServiceLossTimeoutMilliseconds` | Time without device data before the Device Service is lost. |
| `PowerControlLossTimeoutMilliseconds` | Time without a Power Control heartbeat before it is lost. |
| `OperatingModeLossTimeoutMilliseconds` | Time without an Operating Modes heartbeat before it is lost. |
| `AlarmServiceLossTimeoutMilliseconds` | Time without an Alarms heartbeat before it is lost. |


## Luna Web Runtime Configuration
Luna Web reads `config.json` at startup. In Docker it is written from environment variables by [docker/40-app-config.sh](../paskalON.Luna.Web/docker/40-app-config.sh), so the same image can be used in every environment.

| Variable | Default | Description |
|----------|---------|-------------|
| `LUNA_API_URL` | – | Address of the Luna Web API as seen from the nginx container. |
| `APP_API_BASE_URL` | `/api` | Base path of the Web API as seen from the browser. |
| `APP_SYSTEM_HUB_URL`, `APP_DEVICE_HUB_URL`, `APP_DEPENDENCY_HUB_URL`, `APP_ALARM_HUB_URL` | `/hubs/...` | SignalR hub paths. |
| `APP_GRAFANA_URL` | `http://localhost:41006` | Grafana as seen from the browser. |
| `APP_GRAFANA_*` | – | Grafana embedding options (auth mode, org, theme, kiosk, logo, auto fit). |
| `APP_GRAFANA_DASHBOARDS` | System Meter Overview, System Operating Mode Control | Embeddable dashboards as JSON array of name and uid. |
| `APP_DASHBOARD_SECTION_DASHBOARDS`, `APP_DASHBOARD_SECTION_DEFAULT` | System Meter Overview | Dashboards of the Dashboard section and the one selected at start. |

New settings are added to `AppConfig` and `defaultConfig` in `src/api/config.ts` and to the Docker config script.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
