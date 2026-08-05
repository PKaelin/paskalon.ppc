# Device Service
This solution is to self contain all logic regarding the implementation of a Device Service.
The service acts as a translator between a power controller and the physical devices (inverters, battery banks, etc.)
*Note: paskalON.Common libraries are project references during initial development*  


# Device Service Overview
![Device Service Overview.drawio](./Docs/Device%20Service%20Overview.drawio.svg)


## Structure


### Project .Domain.Configs
Persistent Domain Configuration layer of DDD described in [README.md](./paskalON.Devices.Domain.Configs/README.md)


### Project .Domain
Domain layer of DDD described in [README.md](./paskalON.Devices.Domain/README.md)


### Project .Equipments
Equipment and manufacturer specific implementation described in [README.md](./paskalON.Devices.Equipments/README.md)


### Project .Infrastructure
Infrastructure layer of DDD described in [README.md](./paskalON.Devices.Infrastructure/README.md)


---

TODO: Add general add readmes for not yet created projects below.


### Project .Application
Application layer of DDD described in [README.md](./paskalON.Devices.Application/README.md)


### Project .Service
Service/Interface layer described in [README.md](./paskalON.Devices.Service/README.md)


## Common design

### Dataface
The dataface registration pattern is used for loose coupling the domains from the actual communications.
At this point only Modbus and C37 communications are supported but the dataface should make it easy to add 
additional communications in the future.


### Dataface registration design overview

![Dataface Registration Design Overview](./Docs/Dataface%20Registration%20Design%20Overview.drawio.svg)


### Dataface data design overview
The getting of the data is outsourced to an engine. Both the equipment and the engine have a IClient and an IDataface injected to achieve loose coupling.
Below is an abstract design of the idea. 
The ModbusEquipment could be a battery bank, power conversion system, etc. 
The C37Equipment could be a system power meter, circuit power meter, etc.

![Dataface Data Design Overview](./Docs/Dataface%20Data%20Design%20Overview.drawio.svg)


## More information
[See project readmes in structure section](./README.md#structure)


---

## Not implemented at this point
- Application layer
- Service layer
- Generic Modbus Device writes.
- Power meter Modbus communication.
- Battery Bank Racks, Modules, Cells.
- PCS and BB heartbeat and watchdog.
- Check health of the devices.
- Infrastructure repositories
- Infrastructure layer tests
- Modbus library
- C37 library
- Harden Modbus communication (Commands, Communication Errors, etc.)
- Harden C37 communication (Commands, Communication Errors, etc.)