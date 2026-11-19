# Operating Modes Overview
An Operating Mode defines the specific behavior and control strategy the plant uses to interact with the power grid.
The Operating Modes service calculates the plant-level and DER unit power setpoints from the configured operating modes and publishes them to the Power Control.

Back to the [solution README](../README.md).

![Operating Mode Overview](./Assets/Diagrams/Operating%20Mode%20Overview.drawio.svg)


## Responsibilities
- Load the operating mode configuration of one plant type (e.g. BESS, Solar) from its database.
- Manage the operating mode stack and the override slot (add, move, remove, replace, activate, deactivate).
- Calculate the objectives from the stack, ramp the applied targets and enforce the mandatory constraints (nameplate, plant-wide overrides).
- Publish the current system and DER unit setpoints and its heartbeat.
- Hold the last setpoints while a dependency is lost and resume automatically once all dependencies are back.


## Boundaries
- The Operating Modes service does not command devices. It publishes setpoints and the [Power Control](/paskalON.PowerControls/README.md) distributes them to the devices.
- Device data is received from the [Device Service](/paskalON.DeviceService/README.md) through its client library only.
- Operators change the stack, overrides and setpoints through [Luna](/paskalON.Luna/README.md), which calls the [Web API](./Interfaces.md#web-api).


## Documentation
- [Dependencies](./Dependencies.md): services, infrastructure and libraries the solution depends on.
- [Interfaces](./Interfaces.md): Web API, published and subscribed topics, client library.
- [Domain Model](./DomainModel.md): categories, classifications and types of operating modes.
- [Configuration](./Configuration.md): environment variables, secrets, system and operating mode configuration.
- [Design](./Design/Overview.md): how the stack, transitions, ramps and curves work.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
