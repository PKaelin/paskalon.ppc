# Architecture Instructions


## Architectural Overview
All services follow Domain-Driven Design (DDD) principles and shall maintain clear boundaries between:
- Domain logic
- Application/use-case orchestration
- Infrastructure concerns
- External interfaces


## Microservice Boundaries
A service owns:
- Its domain model
- Its business rules
- Its application/use-case logic
- Its persistence model
- Its data
- Its public API/events
- Independent deployable

A service must not directly access another service's:
- Database
- Tables
- Repositories
- Domain entities
- Internal application services
- Internal implementation details


## Exemption of above
- Microservice solution in folder paskalON.DeviceSimulator
- Infrastructure solution in folder InfrastructureInternal