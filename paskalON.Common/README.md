# Common
A centralized collection of reusable methods and configurations.

- Classes in this solution shall be thoroughly tested as they are the base of many solutions and products.
- Changes to classes effect every solution and product using these classes.
- Assemblies shall be versioned and published to a package manager.

Purpose, rules and the full documentation of the library are in the [Common Overview](./Docs/Overview.md).


## Structure

| Project | Purpose |
|---------|---------|
| [paskalON.Dataface](./paskalON.Dataface/README.md) | Dataface interfaces and register definitions for Modbus and C37 communication. |
| [paskalON.Domains](./paskalON.Domains/README.md) | Domain base classes, EF Core base configurations and shared domain types. |
| [paskalON.Hosting](./paskalON.Hosting/README.md) | Startup retry while dependencies are not reachable. |
| [paskalON.Infrastructure](./paskalON.Infrastructure/README.md) | Generic EF Core repositories. |
| [paskalON.Infrastructure.PostgreSql](./paskalON.Infrastructure.PostgreSql/README.md) | PostgreSQL Testcontainers fixture for database integration tests. |
| [paskalON.Maths](./paskalON.Maths/README.md) | Curves, interpolation and other mathematical functions. |
| [paskalON.PhysicalUnits](./paskalON.PhysicalUnits/README.md) | Physical quantities (power, energy, voltage, power factor, temperature). |
| [paskalON.Telemetry](./paskalON.Telemetry/README.md) | Metrics publisher. |


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
