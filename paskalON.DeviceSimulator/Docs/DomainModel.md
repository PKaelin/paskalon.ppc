# Device Simulator Domain Model
The simulated grid and devices of the Device Simulator.

Back to the [Overview](./Overview.md).


## Simulated Devices
The simulated devices in `paskalON.DeviceSimulator.Equipments` build on the domain and equipment descriptions of the [Device Service](/paskalON.DeviceService/Docs/DomainModel.md):
- `SimulatedGrid`: grid shared by all simulated devices (e.g. one grid frequency for every PCS and meter).
- Power conversion systems: simulated power response with dead time, first-order lag and slow down (`PowerConversionSystemSimulationBase`).
- Battery banks: state of charge, charge speed and power taper.
- Power meters: measurements streamed over C37.118, with optional power dropouts.


## Simulation Registries
- `SimulationDeviceRegistry`: all simulated devices.
- `SimulationStoreRegistry`: Modbus data stores of the simulated devices.
- `SimulationStreamRegistry`: C37 streams of the simulated power meters.
- `SimulationCustoms`: custom simulation settings (key/value) per device.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
