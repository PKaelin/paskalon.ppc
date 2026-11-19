# Power Control Overview
The Power Control manages and maintains the safety of the equipment.
It receives the setpoints of the Operating Modes service, applies the system and DER unit constraints, distributes the power between the DER units and publishes the power targets to the Device Service.

Back to the [solution README](../README.md).

![Power Control Overview](./Assets/Diagrams/Power%20Control%20Overview.drawio.svg)


## Responsibilities
- Load the power control and constraint configuration from its database.
- Apply the system constraints to the system setpoints of the Operating Modes service.
- Distribute the power between the DER units with the configured distribution strategy.
- Apply the DER unit constraints and publish the power targets of the PCS.
- Publish its heartbeat, so the Operating Modes service can detect a lost Power Control.


## Boundaries
- The Power Control does not decide on the plant setpoints. It receives them from the [Operating Modes](/paskalON.OperatingModes/README.md) service.
- The Power Control does not communicate with the devices. The [Device Service](/paskalON.DeviceService/README.md) writes the power targets to the devices.
- The Constraint Engine resides within the Power Control solution.


## Important Design Notice
- **The first version of the power control constraint engine is not an engine more like uses constraints to transform or solve targets.**
- **The first version of the power control constraint engine is applying constraints sequentially.  
A better approach would be to accumulate them first, then intersect, then project the result so that P&Q are considered together and not sperate.**


## Documentation
- [Dependencies](./Dependencies.md): services, infrastructure and libraries the solution depends on.
- [Interfaces](./Interfaces.md): Web API, published and subscribed topics, client library.
- [Domain Model](./DomainModel.md): power controls, constraints and maps.
- [Configuration](./Configuration.md): environment variables, secrets, system, power control and constraint configuration.
- [Design](./Design/Overview.md): power distribution.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
