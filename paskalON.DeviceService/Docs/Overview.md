# Device Service Overview
The Device Service acts as a translator between the power control and the physical devices (inverters, battery banks, solar, power meters).
It keeps a constant connection to the devices, publishes their data and writes the power targets of the Power Control to the devices.

Back to the [solution README](../README.md).

![Device Service Overview](./Assets/Diagrams/Device%20Service%20Overview.drawio.svg)


## Responsibilities
- Load the DER structure (groups, units, circuits) and devices of the plant from its database.
- Communicate with the devices over Modbus TCP (e.g. PCS, battery banks) and IEEE C37.118 (power meters).
- Publish the core and detail data of the devices.
- Send heartbeats to the PCS and battery banks to keep the connection between controller and device alive.
- Write the power targets received from the Power Control to the PCS.
- Provide the DER structure and device commands (start, stop, standby, connect, maintenance) over the Web API.


## Boundaries
- The Device Service does not decide on power targets. It receives them from the [Power Control](/paskalON.PowerControls/README.md).
- Only the Device Service communicates with the devices network.
- Other services receive the device data through [paskalON.Devices.Client](../paskalON.Devices.Client/README.md) only.
- Device Service can manage groups of energy resources like PCS-1-99, PCS-100-199, etc.


## Documentation
- [Dependencies](./Dependencies.md): devices, services, infrastructure and libraries the solution depends on.
- [Interfaces](./Interfaces.md): Web API, published and subscribed topics, client library.
- [Domain Model](./DomainModel.md): DER structure and devices.
- [Configuration](./Configuration.md): environment variables, secrets, system and device configuration.
- [Design](./Design/Overview.md): dataface and equipment implementation.


## Not Implemented At This Point
- Generic Modbus Device writes.
- Power meter Modbus communication.
- Battery Bank Racks, Modules, Cells.
- PCS and BB heartbeat and watchdog.
- Check health of the devices.
- Harden Modbus communication (Commands, Communication Errors, etc.)
- Harden C37 communication (Commands, Communication Errors, etc.)


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
