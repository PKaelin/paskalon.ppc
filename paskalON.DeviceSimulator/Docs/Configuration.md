# Device Simulator Configuration
Environment variables, secrets and the simulation configuration of the Device Simulator.

Back to the [Overview](./Overview.md).


## Environment Variables
Set in [docker-compose.yml](../docker-compose.yml).

| Variable | Required | Description |
|----------|----------|-------------|
| `DATABASE_CONNECTION_FILE` | Yes | Secret file with the PostgreSQL connection string of the `devicesimulator` database. |
| `PMU_DATA_RATE` | Yes | Data rate of the simulated C37 power meters (frames per second). |
| `SIMULATION_INTERVAL_MILLISECONDS` | Yes | Interval in which the grid and the device models are advanced. |
| `SIMULATION_PUBLISHING_INTERVAL_MILLISECONDS` | Yes | Interval in which the changed values are pushed to the Device Simulator Web. |

The Device Simulator Web settings (`DER_API_URL`, `APP_...`) are described in [paskalON.DeviceSimulator.Web](../paskalON.DeviceSimulator.Web/README.md).


## Secrets
The secret file for DEV is in [secrets](../secrets): `database_connection`.
How Docker secrets are used is described in [InfrastructureInternal/secrets](/InfrastructureInternal/secrets/README.md).


## Simulation Configuration
The simulated plant uses the same configuration model as the Device Service (see [Device Service Configuration](/paskalON.DeviceService/Docs/Configuration.md#device-configuration)) and is stored in the `devicesimulator` database.
Simulation specific settings are stored as custom configurations (key/value) per device.
The demo plants are loaded by [Client Data](/paskalON.ClientData/README.md) (`paskalON.DeviceSimulator.Data`).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
