# Define
Functional, non-functional, quality, availability, security and other requirements of the Power Plant Controller.

> TBD: Functional, quality and security requirements are not documented yet.


## Failover Requirements
- Failover within the PPC core services shall use a primary and secondary application level concept. The customized protocol shall be less then 25 bytes.
- Failover within the PPC core services from loss of the primary until the secondary publishes its first event shall be <= 500 ms. Docker Swarm takes seconds to reschedule a container, so this target can only be met by the application-level primary/secondary handover. 
- The system accepts rare scenarios where two primary instances are online and concurrently publishing data. However, this condition must be actively monitored and an automated notification must be dispatched upon occurrence.
- Two replicas of one microservice type shall never be on the same node.
- mTLS shall be used between the External and Internal API Gateway communication. That mTLS must have a monitored renewal process.
- Orchestration cluster shall have at least 3 managers.
- Redis Cluster (Sharded + Failover) shall be used instead of Redis Sentinel to minimize failover time.

How these requirements are met is described in the [Failover design](../03-Design/Failover.md).


## Repository Documentation
- [Repository README](/README.md)
- [Documentation Structure And Definition](/Docs/Documentation.md)
