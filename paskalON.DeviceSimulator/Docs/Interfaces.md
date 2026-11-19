# Device Simulator Interfaces
APIs, SignalR hub and simulated device endpoints of the Device Simulator solution.

Back to the [Overview](./Overview.md).


## Web API
Base address in DEV: `http://localhost:45500`. Requests are in the REST Client file [paskalON.DeviceSimulator.Service.http](../paskalON.DeviceSimulator.Service/paskalON.DeviceSimulator.Service.http).


### DER (`/api/v1/der/...`)

| Method | Action | Description |
|--------|--------|-------------|
| GET | `getder` | Gets the DER root with all its contents. |
| POST | `setexpanded` | Sets the expanded lists of the Device Simulator Web tree. |


### Simulation (`/api/v1/simulation/...`)

| Method | Action | Description |
|--------|--------|-------------|
| POST | `setevents` | Sets the warning, fault and vendor event bitmasks of a simulated PCS or battery bank. |
| POST | `setcommunication` | Starts or stops answering Modbus requests of a simulated PCS or battery bank or streaming the C37 data of a power meter. |
| POST | `setpowerdropout` | Switches the random dropout of the power updates of a simulated system power meter on or off. |
| POST | `slowdownpower` | Slows down the active and reactive power response of a simulated PCS. |
| POST | `setresponsedynamics` | Sets the dead time and the first-order lag of the power response of a simulated PCS. |
| POST | `setstateofcharge` | Sets the preferred state of charge of a simulated battery bank. |
| POST | `setchargespeed` | Sets how much faster a simulated battery bank charges and discharges than its physical definition. |
| POST | `setpowertaper` | Enables or disables the power taper of a simulated battery bank near empty and full. |


## SignalR Hub

| Hub | Route | Client methods | Description |
|-----|-------|----------------|-------------|
| `DerHub` | `/hubs/der` | `UnitsUpdated`, `DevicesUpdated` | Pushes the changed units and device values to the Device Simulator Web. |


## Simulated Device Endpoints
The simulator hosts one server per configured device endpoint (address and port from the simulation database):
- **Modbus TCP servers** for PCS and battery banks.
- **IEEE C37.118 servers** for the power meters.

The [Device Service](/paskalON.DeviceService/Docs/Interfaces.md) connects to these endpoints as it would to physical devices.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
