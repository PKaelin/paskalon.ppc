# paskalON.Alarms.Data
Console application that imports a client project data (CSV) file into the Alarms database. **The database is deleted and recreated.**

Part of the [Client Data solution](../README.md).


## Usage
```
paskalON.Alarms.Data ClientProjectName ClientProjectFile [SchemaVersion]
```
Usually started with [ImportClientData.bat](../ImportClientData.bat). The arguments and exit codes are described in [Interfaces](../Docs/Interfaces.md).


## Client Data Files
The client project data files are in [ClientData](./ClientData). The connection string `DatabaseContext` is in `appsettings.json`.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
