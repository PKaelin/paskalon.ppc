# Simulation Cycle
How the Device Simulator advances the simulation and serves the simulated values.

Back to the [Design Overview](./Overview.md).


## Startup
`DeviceManagerSimulator` loads the simulated plant from the database, creates the simulated devices with their models and starts one Modbus or C37 server per configured endpoint.


## Cycle
1. `SimulationWorker` advances the simulated grid and all registered device models every `SIMULATION_INTERVAL_MILLISECONDS`.
2. The Modbus data stores and C37 streams are updated with the new values. The Device Service reads them as from physical devices.
3. `ModbusPollService` polls the simulated Modbus servers with the Modbus polling engines of the Device Service equipments.
4. `SimulationPublisher` pushes the changed values to the Device Simulator Web over the `DerHub` every `SIMULATION_PUBLISHING_INTERVAL_MILLISECONDS`.


## Test Conditions
Test conditions such as events, communication loss, power dropouts or a slow power response are injected over the [Simulation Web API](../Interfaces.md#simulation-apiv1simulation).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
