# paskalON.Alarms.Service
ASP.NET Core host of the Alarms service: Web API controllers, alarm handler and delay workers, alarm action outbox publisher and heartbeat. It is built as a Docker image.

Part of the [Alarms solution](../README.md).


## Web API
The endpoints are described in [Interfaces](../Docs/Interfaces.md#web-api). Requests are in the REST Client file [paskalON.Alarms.Service.http](./paskalON.Alarms.Service.http).


## Configuration
Environment variables and secrets are described in [Configuration](../Docs/Configuration.md).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
