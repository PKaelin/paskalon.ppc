# Power Control

## Power Control Overview
![Power Control Overview](./Docs/Power%20Control%20Overview.drawio.svg)


## Structure

### Project ConstraintEngine.Domain.Configs
Persistent Domain Configuration layer of DDD described in [README.md](./paskalON.ConstraintEngine.Domain.Configs/README.md)

### Project ConstraintEngine.Domain
Domain layer of DDD described in [README.md](./paskalON.ConstraintEngine.Domain/README.md)

### Project PowerControl.Domain.Configs
Persistent Domain Configuration layer of DDD described in [README.md](./paskalON.PowerControls.Domain.Configs/README.md)

### Project PowerControl.Domain
Domain layer of DDD described in [README.md](./paskalON.PowerControls.Domain/README.md)

---

TODO: Add general add readmes for not yet created projects below.

### Project .Application
Application layer of DDD described in [README.md](./paskalON.OperatingModes.Application/README.md)


### Project .Service
Service/Interface layer described in [README.md](./paskalON.OperatingModes.Service/README.md)

---


## High level concept
- SystemConstraint: Is a constraint applied to the whole system.
- PowerControlManager: Is the system manager. It is the main entry point for the power control domain. It is responsible for managing the constraints and maps and providing an interface for the application layer to interact with the power control domain.
- DerConstraint: Is a constraint applied to a single DER.
- PowerControlDer: Is applied to a single DER. It is responsible for managing the power control of the DER.
- PowerControlDerMap: Is a mapper between the power control DER and the physical representation of the DER from the device service. The individual power control DER maps are used in the PowerControlManager to distribute the targets.

![Power Control High Level Concept](./Docs/Power%20Control%20High%20Level%20Concept.drawio.svg)



TODO: Add information.