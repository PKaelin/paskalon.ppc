# Client Data Overview
Client Data loads client data or sample data into the microservice databases.
Each microservice type has one console application that reads a CSV file of a client project and creates the domain configuration defined in that file.

Back to the [solution README](../README.md).


## How It Works

- All client data projects are in one solution.
    - ./paskalON.ClientData/paskalON.ClientData.slnx
    - Each project is in its own subfolder of ./paskalON.ClientData.
    - In the initial phase all required project references in the namespace paskalON are under a folder named: "InitialDevelopment" instead of nuget packages.

- The common project contains common functionalities like CSV parsing, Schema version selection, Property type conversion, etc.
    - paskalON.ClientData.Common.csproj
    - The common project is a class library that is referenced by all other Data projects.

- Each microservice type has one project that reads in a file and creates the data defined in that file.
    - paskalON.Devices.Data.csproj
    - paskalON.DeviceSimulator.Data.csproj
    - paskalON.PowerControls.Data.csproj
    - paskalON.OperatingModes.Data.csproj
    - paskalON.Alarms.Data.csproj
- Each of the Data projects is a console application.


## Boundaries
- Client Data is a development and commissioning tool, not a running service.
- **Important:** the database of every imported service is deleted and recreated.
- It writes the configuration through the infrastructure projects of the target services, which is an exception to the microservice boundary rules.


## Documentation
- [Dependencies](./Dependencies.md): target services, infrastructure and libraries.
- [Interfaces](./Interfaces.md): input arguments, import script and exit codes.
- [Domain Model](./DomainModel.md): domain entities created by the import.
- [Configuration](./Configuration.md): database connection and client project data file location.
- [Design](./Design/Overview.md): CSV file definition and alarms client data.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
