# Power Plant Controller (PPC)
This project is not yet an actual power plant controller.  
# !!!This repo is under initial development!!!


## Core Power Control Overview
![test](./Core%20Power%20Control%20Controller%20Overview.drawio.svg)


## Core Power Control Components


### Portfolio
Is a centralized platform that monitors, coordinates and controls multiple Power Plant Controls (PPCs) across different energy sites (Plants).   
[See Portfolio details](./paskalON.Portfolio/README.md)  


### Plant Control
Is a Power Plant Controller that manages an energy facility.
- Plant can be different locations like Hawaii, Oregon, etc.
- Plant can be different types like BESS, Solar, Nuclear, etc.
- Plant can be scaled like BESS1, BESS2, etc.

[See Plant Control details](./paskalON.PlantControls/README.md)


### Operating Mode
Is a specific configuration and control how the plant produces energy.
- Operating Mode per location, type, scaled.

[See Operating Mode details](./paskalON.OperatingModes/README.md)


### Power Control 
Manages and maintains the safety of equipment.
- Gets configuration from one or more Device component(s) and manages the power distribution.

[See Power Control details](./paskalON.PowerControls/README.md)


### Constraint Engine 
Constraints the targets to phisycal, operational and regulatory constraints. This component resides within the power control solution.


### Device Service 
Manages physical devices and keeps a constant connection to the devices.
- Device Service can manage groups of energy resources like PCS-1-99, PCS-100-199, etc. 

[See Device Service details](./paskalON.DeviceService/README.md)


### Device
The actual devices (power resource) which can be a simulation, emulation or physical representation.



*Please note the Monorepo for a small early development.*  
*Use Multirepo if you have multiple independent teams or for better separation.*

---
TODO: Decide whether to include and document:  
*Telemetry, User Interfaces, Alarms, Monitoring, Outstation, DeviceWarrantyService, API Gateways, DMZ (In/Out) Components, Deployment Components.*