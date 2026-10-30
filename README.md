# Power Plant Controller (PPC)
**This repository is source-available and not open source, with commercial/production use requiring a separate license!**

## !!!This repo is under initial development!!!
This project is not yet an actual power plant controller.  


## Core Power Control Overview
![test](./Docs/Core%20Power%20Control%20Controller%20Overview.drawio.svg)


## Network and Deployment Architecture Overview
![Network and Deployment Architecture Overview](./Docs/Network%20and%20Deployment%20Architecture%20Overview.svg)  
*Overview does not show platform and infrastructure services*


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
Constraints the targets to physical, operational and regulatory constraints. This component resides within the power control solution.


### Device Service 
Manages physical devices and keeps a constant connection to the devices.
- Device Service can manage groups of energy resources like PCS-1-99, PCS-100-199, etc. 

[See Device Service details](./paskalON.DeviceService/README.md)


### Device
The actual devices (power resource) which can be a simulation, emulation or physical representation.


## Common design

### Configurations
Configurations are stored in a database to ensure accountability and to track changes.


### Config - Domain - Application Overview
PlantControl, OperatingMode, PowerControl, DeviceService are all setup using the same concept. The configuration and structure of the microservice is stored in a database. During startup the configuration is read and factories create the domains and domain structure passing in the domain mappers that reference the subscribed data.


![Config-Domain-Application Overview](./Docs/Config-Domain-Application%20Overview.drawio.svg)


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


## Fault Tolerance
For detailed setup and fault tolerance procedures see the [FAULT TOLERANCE](./FAULT_TOLERANCE.md)

![Fault Tolerance](./Docs/Fault%20Tolerance.drawio.svg)


## Failover 
For detailed setup and recovery procedures see the [FAILOVER](./FAILOVER.md)

![Failover](./Docs/Failover.drawio.svg)

---

*Please note the Monorepo for a small early development.*  
*Also note that all the solutions contain a folder initial develpment which one day will be replaced with nuget packages.*
*Use Multirepo if you have multiple independent teams or for better separation.*

---
TODO: Decide whether to include and document:  
*Telemetry, User Interfaces, Alarms, Monitoring, Outstation, DeviceWarrantyService, API Gateways, DMZ (In/Out) Components, Deployment Components.*