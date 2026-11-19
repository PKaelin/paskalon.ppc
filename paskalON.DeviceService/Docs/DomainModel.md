# Device Service Domain Model
The DER structure and devices of the Device Service.

Back to the [Overview](./Overview.md).


## Domain
Most of the domains have a related configuration, see [Configuration](./Configuration.md#device-configuration).
The structure (Group, Unit, Circuit, etc.) and devices (BB, PCS, Meter, etc.) will only be apparent once the domain configurations are loaded.

![DeviceService Domain](./Assets/Diagrams/DeviceService%20Domain.drawio.svg)


## Structure
- `Der`: root of the plant.
- `DerGroup`, `DerCircuit`, `DerUnit`: logical groups of devices.
- `DerBatteryStorageUnit`, `DerSolarUnit`: units with their power conversion system and energy resource or storage.


## Devices
- Power conversion systems (PCS).
- Energy storages: battery banks.
- Energy resources: solar panels.
- Meters: system, circuit, external and auxiliary power meters.
- Generic Modbus devices (GMD).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
