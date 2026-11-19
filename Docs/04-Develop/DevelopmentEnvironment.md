# Development Environment
Tools and steps to set up and run the Power Plant Controller on a developer machine.


## Tools
- Visual Studio 2026 (version 18) with the ASP.NET, Docker and Node.js workloads.
- .NET 10 SDK (all projects target `net10.0`).
- Docker Desktop.
- Node.js 20.19 or later for the web projects (Luna Web, DeviceSimulator Web).
- Entity Framework Core tools (`dotnet-ef`), see [Database Migrations](./DatabaseMigrations.md).


## Start The Infrastructure
The platform services (PostgreSQL, RabbitMQ, Redis, telemetry, Grafana) run in Docker. See [InfrastructureInternal](/InfrastructureInternal/README.md).
```
cd InfrastructureInternal
docker compose up -d
```


## Open The Solutions
[Start PPC DevEnv.bat](/Start%20PPC%20DevEnv.bat) opens all solutions in Visual Studio, opens a command prompt for the client data import and starts Docker Desktop.
The paths to `devenv.exe` and the repository are set at the top of the script and may need to be adjusted.


## Load Client Data
[Client Data](/paskalON.ClientData/README.md) loads client or sample data into the microservice databases (`paskalON.ClientData/ImportClientData.bat`).


## Repository Documentation
- [Development Overview](./Overview.md)
- [Repository README](/README.md)
