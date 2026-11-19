# Device Simulator Dependencies
Projects, infrastructure and libraries the Device Simulator solution depends on.

Back to the [Overview](./Overview.md).


## Services

| Service | Direction | How | Purpose |
|---------|-----------|-----|---------|
| [Device Service](/paskalON.DeviceService/README.md) | Consumer | Modbus TCP and IEEE C37.118 | Communicates with the simulated devices as with physical devices. |


## Infrastructure
All provided by [InfrastructureInternal](/InfrastructureInternal/README.md) in DEV and TST.

| Infrastructure | Purpose |
|----------------|---------|
| PostgreSQL (`devicesimulator` database) | Simulated plant: DER structure, devices, endpoints and simulation settings. |


## Libraries

| Library | Solution | Purpose |
|---------|----------|---------|
| paskalON.Dataface | [Common](/paskalON.Common/README.md) | Dataface types used by the DTOs. |
| paskalON.Protocols.Modbus | [Communication](/paskalON.Communication/README.md) | Modbus TCP server. |
| paskalON.Protocols.C37118 | [Communication](/paskalON.Communication/README.md) | IEEE C37.118 server (PMU simulation). |

The Device Simulator Web uses Vue 3, TypeScript, Vite, Bootstrap 5 and the SignalR JavaScript client, see [paskalON.DeviceSimulator.Web](../paskalON.DeviceSimulator.Web/README.md).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
