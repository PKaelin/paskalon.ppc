# Client Data Configuration
Database connection and client project data file location.

Back to the [Overview](./Overview.md).


## Database Connection
Each Data project reads the connection string `DatabaseContext` from its `appsettings.json`. The ports and database names of the DEV databases are listed in [InfrastructureInternal](/InfrastructureInternal/README.md#databases-datayaml).


## Client Project Data (CSV) File Location
- The client project data files shall be in a folder within the client project folder.
- The root data folder shall be named "ClientData" and the subfolders shall be named after the specific project or "Demo" if its demo/development configuration.
    - E.g. Devices: ClientData\Demo\DemoSuperSimpleBattery.csv or ClientData\TheClient\ClientSite.csv
    - E.g. DeviceSimulator: ClientData\Demo\DemoSuperSimpleBattery.csv or ClientData\TheClient\ClientSite.csv
    - E.g. PowerControls: ClientData\Demo\DemoSuperSimpleBattery.csv or ClientData\TheClient\ClientSite.csv
    - E.g. OperatingModes: ClientData\Demo\DemoSuperSimpleBattery.csv or ClientData\TheClient\ClientSite.csv
    - E.g. Alarms: ClientData\Demo\DemoSuperSimpleBattery.csv or ClientData\TheClient\ClientSite.csv



The demo client projects are `DemoSuperSimpleBattery`, `DemoSimpleBattery`, `DemoBattery`, `DemoSuperSimpleSolar` and `DemoSolar`.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
