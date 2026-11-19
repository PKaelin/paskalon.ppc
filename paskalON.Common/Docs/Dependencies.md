# Common Dependencies
Which solutions use the Common libraries and which external packages the libraries depend on.

Back to the [Overview](./Overview.md).


## Used By

| Library | Used by |
|---------|---------|
| paskalON.Dataface | Device Service, Device Simulator, Communication (protocols) |
| paskalON.Domains | Device Service, Power Control, Operating Modes, Alarms, Client Data |
| paskalON.Hosting | Power Control, Operating Modes, Alarms, Luna |
| paskalON.Infrastructure | Device Service, Power Control, Operating Modes, Alarms |
| paskalON.Infrastructure.PostgreSql | Integration test projects with a database |
| paskalON.Maths | Device Service, Operating Modes |
| paskalON.PhysicalUnits | Device Service, Power Control, Operating Modes, Luna |
| paskalON.Telemetry | Device Service, Power Control, Operating Modes, Alarms, Luna |


## External Packages

| Library | Packages |
|---------|----------|
| paskalON.Domains | Microsoft.EntityFrameworkCore, Npgsql.EntityFrameworkCore.PostgreSQL |
| paskalON.Hosting | Microsoft.Extensions.Logging.Abstractions |
| paskalON.Infrastructure.PostgreSql | MSTest.TestFramework, Npgsql.EntityFrameworkCore.PostgreSQL, Testcontainers.PostgreSql |
| paskalON.Maths | Microsoft.Extensions.Logging.Abstractions |
| paskalON.Telemetry | Microsoft.Extensions.DependencyInjection, Microsoft.Extensions.Logging.Abstractions |

paskalON.Infrastructure references paskalON.Domains. The other libraries have no project references.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
