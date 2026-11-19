# Common Overview
A centralized collection of reusable methods and configurations shared by the PPC solutions.
Common is a library: it supports one or more solutions and is not independently deployable.

Back to the [solution README](../README.md).


## Rules
- Classes in this solution shall be thoroughly tested as they are the base of many solutions and products.
- Changes to classes effect every solution and product using these classes.
- Assemblies shall be versioned and published to a package manager. During initial development they are project references.
- Common contains no business logic of a specific service.


## Libraries

| Library | Purpose |
|---------|---------|
| paskalON.Dataface | Dataface interfaces and register definitions for Modbus and C37 communication. |
| paskalON.Domains | Base classes of the domain entities, EF Core base configurations and shared domain types. |
| paskalON.Hosting | Startup retry while dependencies are not reachable. |
| paskalON.Infrastructure | Generic EF Core repositories. |
| paskalON.Infrastructure.PostgreSql | PostgreSQL Testcontainers fixture for database integration tests. |
| paskalON.Maths | Curves, interpolation, exponential and logarithmic functions, sums and random walkers. |
| paskalON.PhysicalUnits | Physical quantities: power, energy, voltage, current, power factor, temperature, state of charge. |
| paskalON.Telemetry | Metrics publisher. |


## Documentation
- [Dependencies](./Dependencies.md): which solutions use which library and the external packages.
- [Interfaces](./Interfaces.md): the main public types of each library.
- [Domain Model](./DomainModel.md): domain base classes and shared domain types.
- [Configuration](./Configuration.md): configuration of the libraries.
- [Design](./Design/Overview.md): designs of the libraries.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
