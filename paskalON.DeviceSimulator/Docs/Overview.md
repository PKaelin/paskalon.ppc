# Device Simulator Overview
The Device Simulator simulates the physical devices of a plant (PCS, battery banks, power meters and the grid) for development and test.
It hosts the Modbus TCP and IEEE C37.118 endpoints the [Device Service](/paskalON.DeviceService/README.md) connects to, so the PPC can run without physical devices.

Back to the [solution README](../README.md).


## Responsibilities
- Load the simulated plant (DER structure, devices and their communication endpoints) from its database.
- Host one Modbus server or C37 server per configured device endpoint.
- Advance the simulated grid and all device models at a fixed interval (power response, state of charge, measurements).
- Inject test conditions over the Web API: events, communication loss, power dropouts, slow or delayed power response, state of charge, charge speed and power taper.
- Show and edit the simulated values in the Device Simulator Web user interface.


## Boundaries
- The Device Simulator is a development and test tool and is never deployed to a plant.
- It is exempt from the microservice boundary rules (see [architecture instructions](/.ai/core/architecture.instructions.md#exemption-of-above)): it reuses the domain and equipment projects of the Device Service.


## Documentation
- [Dependencies](./Dependencies.md): projects, infrastructure and libraries the solution depends on.
- [Interfaces](./Interfaces.md): Web API, SignalR hub and simulated device endpoints.
- [Domain Model](./DomainModel.md): simulated grid and devices.
- [Configuration](./Configuration.md): environment variables, secrets and simulation configuration.
- [Design](./Design/Overview.md): simulation cycle.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
