# System Deployment
Networks, containers and orchestration of the Power Plant Controller.

![Network and Deployment Architecture Overview](../Assets/Diagrams/Network%20and%20Deployment%20Architecture%20Overview.svg)  
*Overview does not show platform and infrastructure services*


## Networks
- **PPC-DMZ:** hosts Luna Web, the Luna API services, Grafana Web and the External API Gateway.
- **Core-Control network:** hosts the PPC core services (Plant Control, Operating Mode, Power Control, Device Service) and the platform services.
- **Devices network:** hosts the physical devices. Only the Device Service sends data to it.

Each network has its own Docker Swarm cluster. Replicas, gateways and shared state are described in [Failover - High Availability](../03-Design/Failover.md#high-availability-ha-definition).


## Development and Test Infrastructure
In DEV and TST the platform services (databases, message broker, cache, telemetry) run with Docker Compose from [InfrastructureInternal](/InfrastructureInternal/README.md).


## Repository Documentation
- [Architecture Overview](./Overview.md)
- [Repository README](/README.md)
