# Communication Configuration
The Communication libraries have no configuration of their own. They are configured by the solutions that use them.

Back to the [Overview](./Overview.md).


## Configured By The Using Solutions
- Redis and RabbitMQ connection strings: secret files of the services (`MESSAGING_CONNECTION_FILE`, `RABBITMQ_CONNECTION_FILE`).
- Topics, heartbeat intervals and loss timeouts: system configuration of the services.
- Modbus and C37 connections (`ClientConnectionConfig`): device configuration of the Device Service.


## Client Connection Configuration
`ClientConnectionConfig` of the Modbus and C37 clients:

| Property | Description |
|----------|-------------|
| `ServerAddress`, `ServerPort`, `AddressFamily` | Server the client connects to (default IPv4). |
| `ConnectionTimeoutMilliseconds`, `DisconnectionTimeoutMilliseconds` | Wait time for a successful connect or disconnect before raising an error. |
| `ConnectRetryCount`, `ConnectRetryIntervalMilliseconds` | Reconnect attempts (-1 endless, 0 connects once) and the wait time between them. |
| `OperationTimeoutMilliseconds` | Timeout for sending or reading a response (-1 no timeout). |
| `SendRetryCount`, `SendRetryIntervalMilliseconds` | Retries of a failed send (Modbus only). |


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
