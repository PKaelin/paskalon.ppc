# paskalON.Luna.Web
Luna Web: single page application (Vue 3, TypeScript, Bootstrap 5, built with Vite) of the operator user interface. It talks only to the Luna Web API (REST and SignalR) and embeds the Grafana dashboards.

Part of the [Luna solution](../README.md).


## Views
- Dashboard: system values and embedded Grafana dashboards.
- System: DER tree with the devices and their commands.
- Operating Modes: operating mode stack, overrides and setpoints.
- Alarms: alarm list, filters, details and acknowledgement.
- Settings.


## Development
```
npm install
npm run dev
npm run build
```
The rules for components, real-time data and value formatting are in the [user interface instructions](/.ai/core/user-interface.instructions.md).


## Docker
The image serves the built application with nginx and proxies `/api` and `/hubs` to the Luna Web API. The runtime configuration is written from environment variables at container start, see [Configuration](../Docs/Configuration.md#luna-web-runtime-configuration).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
