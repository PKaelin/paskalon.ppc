# Device Simulator
This solution simulates the physical devices of a plant (PCS, battery banks, power meters and the grid) for development and test.
The Device Service communicates with the simulated devices over Modbus TCP and IEEE C37.118 as it would with physical devices.

Purpose, responsibilities and the full documentation of the solution are in the [Device Simulator Overview](./Docs/Overview.md).


## Structure

| Project | Purpose |
|---------|---------|
| [paskalON.DeviceSimulator.Equipments](./paskalON.DeviceSimulator.Equipments/README.md) | Simulated devices, grid and simulation registries. |
| [paskalON.DeviceSimulator.Application](./paskalON.DeviceSimulator.Application/README.md) | Simulator device manager that hosts the Modbus and C37 servers. |
| [paskalON.DeviceSimulator.Dto](./paskalON.DeviceSimulator.Dto/README.md) | DTOs of the Device Simulator. |
| [paskalON.DeviceSimulator.Service.Dto](./paskalON.DeviceSimulator.Service.Dto/README.md) | Request DTOs of the Web API. |
| [paskalON.DeviceSimulator.Service](./paskalON.DeviceSimulator.Service/README.md) | Web API, SignalR hub, simulation worker and publisher. |
| [paskalON.DeviceSimulator.Web](./paskalON.DeviceSimulator.Web/README.md) | Device Simulator Web single page application (Vue 3). |

*Note: This solution is exempt from the microservice boundary rules and references Device Service projects directly.*


## Docker Compose
[docker-compose.yml](./docker-compose.yml) builds and starts the Device Simulator service container (`paskalon.devicesimulator.service`, port 45500) and the Device Simulator Web container (`paskalon.devicesimulator.web`, port 45502) in the network `ppc.network`.
The database runs in [InfrastructureInternal](/InfrastructureInternal/README.md), which must be started first.


## Web API
The Web API provides the simulated DER structure and injects test conditions (events, communication loss, power dropouts, power response, state of charge). The endpoints are described in [Interfaces](./Docs/Interfaces.md#web-api).
REST Client file: [paskalON.DeviceSimulator.Service.http](./paskalON.DeviceSimulator.Service/paskalON.DeviceSimulator.Service.http).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
