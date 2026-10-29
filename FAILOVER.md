# Failover 

## Definitions
- PPC core services: Plant Control (controls plants), Operating Mode (controls modes and setpoints), Power Control (controls constraints), Device Service (controls devices)
- PPC-DMZ (Power Plant Control Demilitarized Zone): A network that functions like a standard DMZ but is isolated specifically for power plant control systems.
- Core-Control network: A network for all PPC core services and platform services. 
- Docker Swarm serves as the primary container orchestration platform, responsible for clustering Docker hosts and automating container lifecycle management.


## Failover Requirements
- Failover within the PPC core services shall use a primary and secondary application level concept. The customized protocol shall be less then 25 bytes.
- Failover within the PPC core services from loss of the primary until the secondary publishes its first event shall be <= 500 ms. Docker Swarm takes seconds to reschedule a container, so this target can only be met by the application-level primary/secondary handover. 
- The system accepts rare scenarios where two primary instances are online and concurrently publishing data. However, this condition must be actively monitored and an automated notification must be dispatched upon occurrence.
- Two replicas of one microservice type shall never be on the same node.
- mTLS shall be used between the External and Internal API Gateway communication. That mTLS must have a monitored renewal process.
- Orchestration cluster shall have at least 3 managers.
- Redis Cluster (Sharded + Failover) shall be used instead of Redis Sentinel to minimize failover time.


## Failover Strategy
- PPC core services shall have one primary and one secondary of each service type.
- Primary and secondary communicate, through a customized protocol and logic, with each other to identify which instance is primary.
- Only the primary publishes or sends requests to the next service/device. 
- The secondary only subscribes to published events but does not publish or send any data unless it becomes the primary.


### Primary and secondary definition

| Capability | Primary | Secondary |
|------------|---------|-----------|
| Subscribe to upstream events | Yes | Yes |
| Maintain a current state replica | Yes | Yes |
| Validate incoming messages | Yes | Yes |
| Compute candidate control results | Yes |Yes |
| Publish control events | Yes | No |
| Issue device commands | Yes | No, only keep alive to devices |
| Participate in leadership election | Yes | Yes |


## High Availability (HA) definition


### HA in PPC-DMZ network
- PPC-DMZ network shall have its own Docker swarm cluster.
- Luna Web (user interface) shall run in Docker containers in Docker swarm with 2 replicas.
- Luna API services for the web site shall run in Docker containers in Docker swarm with 2 replicas.
- External API Gateway (HAProxy) the gateway for all the request to the Core-Control network shall run in Docker containers in Docker swarm with 2 replicas. 
- Luna API service will communicate over the API Gateway to the backend services that are hosted in the Core-Control network (secure network)
- Luna API states shall be written to a shared cluster cache (Redis). When a new Luna API service is started it shall read the current Luna API states.
- Monitor Web (user interface) shall run in Docker containers in Docker swarm with 2 replicas.
- Upload service has only one instance with a periodic upload interval. The process must be logged and in the event of a missing interval an alert must be triggered. Downtime is accepted but once alert is triggered needs an imediate reaction.


### HA in Core-Control network
- Plant Control, Operating Mode, Power Control, Device Service shall be hosted in the Core-Control network.
- Core-Control network shall have its own Docker swarm cluster.
- All services in the Core-Control network shall run in Docker containers in Docker swarm.
- Internal API Gateway (HAProxy also 2 replicas) and all requests from the API Gateway in the PPC-DMZ network go through this Gateway for security reasons.
- Every service within the Core-Control network can communicate with each other.
- Only the Internal API Gateway can receive requests from the PPC-DMZ network and only from the API Gateway using mTLS on both sides.
- Only the Device Service can send data to the Devices network (where the physical devices are hosted)
- Operating Mode states shall be written to a shared cluster cache (Redis). When a new Operating Mode service is started it shall read the current Operating Mode states.
- Device Service states shall be written to a shared cluster cache (Redis). When a new Device Service is started it shall read the current Device Service  states.


TODO: Add HA-Failover details and database failover details