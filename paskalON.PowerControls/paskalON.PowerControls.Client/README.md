# paskalON.PowerControls.Client
Subscriber for all the microservices that need to receive the power targets of the power controls service.

Part of the [Power Control solution](../README.md).


## Usage
Each consuming service implements `IPowerTargetHandler` to decide what to do with a received power target, e.g. the device service applies it to the power conversion system (PCS).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
