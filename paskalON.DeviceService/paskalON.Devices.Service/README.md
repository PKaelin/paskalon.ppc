# paskalON.Devices.Service
ASP.NET Core host of the Device Service: Web API controllers, Modbus polling, C37 streaming, device heartbeat, device data and metrics publishers. It is built as a Docker image.

Part of the [Device Service solution](../README.md).


## Web API
The endpoints are described in [Interfaces](../Docs/Interfaces.md#web-api) and can be called with the REST Client file [paskalON.Devices.Service.http](./paskalON.Devices.Service.http).


## Configuration
Environment variables and secrets are described in [Configuration](../Docs/Configuration.md).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
