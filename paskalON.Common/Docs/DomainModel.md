# Common Domain Model
Domain base classes and shared domain types of `paskalON.Domains`.

Back to the [Overview](./Overview.md).


## Base Classes
- `DomainBase`: base class for all domain entities, providing common properties for tracking changes (`ChangedBy`, `ChangedDate`) and identifying entities.
- `NameBase`: base class for all named domain entities.
- `ConfigurationBase`: general configuration class.
- `History`: defines the table the Entity Framework migration history is stored in.

The EF Core configurations of the base classes are in `paskalON.Domains/Configurations`.


## Shared Types
- `PowerControlType`: type of the plant, a flag of `Bess`, `Solar`, `Wind`, `Nuclear`, `Hydro` and `Gas`.
- `IValueGetter`, `IValueSetter`: contracts to read and write values.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
