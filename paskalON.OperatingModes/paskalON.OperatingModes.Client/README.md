# paskalON.OperatingModes.Client
Subscriber for all the microservices that need to receive the power setpoints of the operating modes service.

Part of the [Operating Modes solution](../README.md).


## Usage
Each consuming service implements `IPowerSetpointHandler` to decide what to do with received system and DER unit power setpoints.
E.g. the power controls service applies them to its power control manager.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
