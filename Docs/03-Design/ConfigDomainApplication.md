# Config - Domain - Application
How the PPC microservices are built from their configuration at startup.


## Configurations
Configurations are stored in a database to ensure accountability and to track changes.


## Startup
PlantControl, OperatingMode, PowerControl, DeviceService are all setup using the same concept. The configuration and structure of the microservice is stored in a database. During startup the configuration is read and factories create the domains and domain structure passing in the domain mappers that reference the subscribed data.

![Config-Domain-Application Overview](../Assets/Diagrams/Config-Domain-Application%20Overview.drawio.svg)


## Repository Documentation
- [Design Overview](./Overview.md)
- [Repository README](/README.md)
