# Power Plant Controller (PPC)
This project is not yet an actual power plant controller.  

---
---
## !!!This repo is under initial development!!!
## Implemented so far
- Implemented partial: C37 protocol, Mobus protocol, Physical Units, Maths, Telemetry, Device infrastructure.
- Implemented metrics publisher [See](#metrics-publisher)
- Implemented dataface [See](./paskalON.DeviceService/README.md#dataface)
- Readme device service: [See](./paskalON.DeviceService/README.md)
- Implemented device service domain: [See](./paskalON.DeviceService/paskalON.Devices.Domain/README.md)
- Implemented device service config: [See](./paskalON.DeviceService/paskalON.Devices.Domain.Configs/README.md)

---
---

## Core Power Control Overview
![test](./Docs/Core%20Power%20Control%20Controller%20Overview.drawio.svg)


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


## Common design

### Metrics publisher
Any class can publish metrics as long as a metrics publisher interface gets injected to the class and an instance (MetricsPublisher) of that injected interface is kept within the class. E.g. 
```
public IMetricsPublisher MetricsPublisher { get; init; }
```

Any class than can register its metric entries via the interface. E.g. 
```
// Step 1 - Initialise metric:
IEnumerable<KeyValuePair<string, object?>> tags = new Dictionary<string, object?>
{
    { "Name", _config.Name },
    { "DeviceId", _config.DeviceId }
};
MetricsPublisher.Initialize("MeasurementName", tags);

// Step 2 - Register one to many metric entries:
MetricsPublisher.Register<Device, double>(this, nameof(Power), MetricType.Gauge, x => x.Power, MetricsFactorClass1);
```

![Metrics Publisher Design](./Docs/Metrics%20Publisher%20Design.drawio.svg)


---

*Please note the Monorepo for a small early development.*  
*Use Multirepo if you have multiple independent teams or for better separation.*

---
TODO: Decide whether to include and document:  
*Telemetry, User Interfaces, Alarms, Monitoring, Outstation, DeviceWarrantyService, API Gateways, DMZ (In/Out) Components, Deployment Components.*