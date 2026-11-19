# paskalON.Devices.Equipments.IntegrationTest
Integration tests of [paskalON.Devices.Equipments](../paskalON.Devices.Equipments/README.md) together with the real domain and protocol classes (simple PCS, battery bank and system power meter proxies).

Part of the [Device Service solution](../README.md).


## Integration Test
The tests run in-memory with the real equipment, domain and protocol classes. The device communication is mocked, so no device, simulator or Docker is required: run them with `dotnet test`.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
