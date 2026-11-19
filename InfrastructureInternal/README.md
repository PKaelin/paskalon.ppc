# Infrastructure Internal
Docker Compose setup of the platform services that the PPC microservices need in DEV and TST.
QA and PRD use Docker stack and orchestration instead.


## Start And Stop
```
docker compose up -d
docker compose down
```
[docker-compose.yaml](./docker-compose.yaml) loads both files below. Each file can also be started on its own, e.g. `docker compose -f data.yaml up -d`.

- [infra.yaml](./infra.yaml): shared platform services.
- [data.yaml](./data.yaml): one PostgreSQL database per microservice.

All containers join the attachable network `ppc.network`, so the microservice containers can reach them.


## Platform Services (infra.yaml)

| Service | Purpose | Host port |
|---------|---------|-----------|
| pgAdmin | Database management UI for all databases | 41001 |
| Redis | Shared cache for service states | 41002 |
| OpenTelemetry Collector | Receives metrics, logs and traces from the services | – |
| Loki | Log storage | 41003 |
| InfluxDB | Metrics storage | 41004 |
| Tempo | Trace storage | 41005 |
| Grafana | Dashboards for metrics, logs and traces | 41006 |
| RabbitMQ | Message broker (AMQP 41007, management UI 41008) | 41007, 41008 |


## Databases (data.yaml)

| Container | Database | Host port |
|-----------|----------|-----------|
| paskalon.devices.data | devices | 45001 |
| paskalon.powercontrols.data | powercontrols | 45011 |
| paskalon.operatingmodes.data | operatingmodes | 45021 |
| paskalon.alarms.data | alarms | 45031 |
| paskalon.devicesimulator.data | devicesimulator | 45501 |

How the database schemas are created is described in [Database Migrations](/Docs/04-Develop/DatabaseMigrations.md).


## Folders
- `grafana/`: provisioned data sources and dashboards.
- `loki/`, `opentelemetry/`, `redis/`, `tempo/`: configuration files of the services.
- `pgadmin/`: preferences and server registrations. Add new databases to `servers.json`.
- [rabbitmq/](./rabbitmq/README.md): exchanges, queues and bindings that are imported at startup.
- [secrets/](./secrets/README.md): Docker secrets with the development credentials.


## Repository Documentation
- [Repository README](/README.md)
- [System Deployment](/Docs/02-Architect/SystemDeployment.md)
- [Development Environment](/Docs/04-Develop/DevelopmentEnvironment.md)
