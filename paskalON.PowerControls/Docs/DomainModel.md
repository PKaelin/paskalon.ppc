# Power Control Domain Model
Power controls, constraints and maps of the Power Control solution.

Back to the [Overview](./Overview.md).


## High Level Concept
- SystemConstraint: Is a constraint applied to the whole system. The same system constraint can be applied to many systems.
- PowerControlManager: Is the system manager. It is the main entry point for the power control domain. It is responsible for managing the constraints and maps and providing an interface for the application layer to interact with the power control domain.
- DerConstraint: Is a constraint applied to a single DER. The same DER constraint can be applied to many DERs.
- PowerControlDer: Is applied to a single DER. It is responsible for managing the power control of the DER.
- PowerControlDerMap: Is a mapper between the power control DER and the physical representation of the DER from the device service. The individual power control DER maps are used in the PowerControlManager to distribute the targets.

![Power Control High Level Concept](./Assets/Diagrams/Power%20Control%20High%20Level%20Concept.drawio.svg)


## Constraint Engine
The constraints are implemented in `paskalON.ConstraintEngine.Domain`:
- Power constraints (`PowerConstraintBase`): limit the active and reactive power.
- Ramp constraints (`PowerRampConstraintBase`): limit the change rate of the power.

Both exist for the system (`ISystemConstraint`) and for DER units (`IDerUnitConstraint`).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
