# Power Plant Controller (PPC)
**This repository is source-available and not open source, with commercial/production use requiring a separate license!**

## !!!This repo is under initial development!!!
This project is not yet an actual power plant controller.  


## What The PPC Does
The operator sets targets, the PPC turns them into setpoints for every device and keeps the plant on target at the grid connection.

![PPC Product Overview](./Docs/Assets/Diagrams/PPC%20Product%20Overview.drawio.svg)


## Solutions
How the solutions of this repository work together. Details are in the [Solution Landscape](./Docs/02-Architect/SolutionLandscape.md).

![PPC Solution Landscape](./Docs/Assets/Diagrams/PPC%20Solution%20Landscape.drawio.svg)

| Solution | Purpose |
|----------|---------|
| [Plant Control](./paskalON.PlantControls/README.md) | Power Plant Controller that manages an energy facility. |
| [Operating Modes](./paskalON.OperatingModes/README.md) | Control strategy the plant uses to interact with the power grid. |
| [Power Controls](./paskalON.PowerControls/README.md) | Power distribution and constraint engine that keep the equipment safe. |
| [Device Service](./paskalON.DeviceService/README.md) | Translator between the power control and the physical devices. |
| [Alarms](./paskalON.Alarms/README.md) | Evaluation and lifecycle of PPC alarms. |
| [Luna](./paskalON.Luna/README.md) | Operator user interface (Luna Web) and its API. |
| [Device Simulator](./paskalON.DeviceSimulator/README.md) | Simulated devices for development and test. |
| [Client Data](./paskalON.ClientData/README.md) | Loads client or sample data into the microservice databases. |


## Libraries

| Library | Purpose |
|---------|---------|
| [Common](./paskalON.Common/README.md) | Reusable domain, hosting, infrastructure, maths, physical units and telemetry code. |
| [Communication](./paskalON.Communication/README.md) | Messaging (RabbitMQ, Redis) and protocols (C37.118, Modbus). |


## Infrastructure
[InfrastructureInternal](./InfrastructureInternal/README.md) runs the platform services (databases, message broker, cache, telemetry) with Docker Compose in DEV and TST.


## Documentation
- [Define](./Docs/01-Define/Overview.md): requirements.
- [Architect](./Docs/02-Architect/Overview.md): solution landscape and deployment.
- [Design](./Docs/03-Design/Overview.md): designs that span multiple solutions.
- [Develop](./Docs/04-Develop/Overview.md): development environment, database migrations and AI instructions.
- [Test](./Docs/05-Test/Overview.md): test levels and how to run the tests.
- [Release](./Docs/06-Release/Overview.md): release process.
- [Documentation Structure And Definition](./Docs/Documentation.md): where documentation lives and how it is kept current.

---

*Please note the Monorepo for a small early development.*  
*Also note that all the solutions contain a folder initial develpment which one day will be replaced with nuget packages.*
*Use Multirepo if you have multiple independent teams or for better separation.*
