# Communication Interfaces
The main public types of the Communication libraries.

Back to the [Overview](./Overview.md).


## paskalON.Messaging
- `IMessagePublisher.PublishAsync(topic, json)`: publishes a JSON message on a topic.
- `IMessageSubscriber.Subscribe(topic, callback)`: subscribes a callback to a topic.
- Heartbeats: `HeartbeatPublisher`, `HeartbeatService`, `HeartbeatSubscriber`, `HeartbeatDto`.
- Sequences: `ISequencedMessage`, `MessageSequencer`, `MessageStamp`, `MessageOrderGuard`.
- Health: `IDependencyHealthMonitor`, `DependencyHealthMonitor`, `DependencyState`.


## paskalON.Messaging.Redis
- `RedisMessagePublisher`, `RedisMessageSubscriber`: Redis Pub/Sub implementations of `IMessagePublisher` and `IMessageSubscriber` (StackExchange.Redis).


## paskalON.Messaging.RabbitMq
- `IRabbitMqPublisher`, `RabbitMqPublisher`: publishes messages to a RabbitMQ exchange.
- `IRabbitMqSubscriber`, `RabbitMqSubscriber`: subscribes to a RabbitMQ exchange.
- `RabbitMqMessage`: JSON message with message ID, routing key and type, published to a RabbitMQ topic exchange.


## paskalON.Messaging.RabbitMq.Testing
- `RabbitMqFixture`: starts and stops the RabbitMQ container once per test assembly.
- `RabbitMqTestRecipient`: receives the published messages in tests.


## paskalON.Protocols.Modbus
- `IModbusClient`, `IModbusServer`: Modbus TCP client and server, implemented with NModbus (`NModbusServer`).
- `MemoryModbusClient`: in-memory client.
- `ClientConnectionConfig`: server address, port, timeouts and retries of a client.


## paskalON.Protocols.C37118
- `IC37Client`, `C37Client`: C37.118 client (PDC side) that receives the data frames of a PMU.
- `IC37Server`, `C37Server`: C37.118 server (PMU side), used by the Device Simulator.
- `ClientConnectionConfig`, `C37ConfigBlueprint`: connection and configuration frame definitions.

The frames are described in [C37.118 Protocol](./Design/C37118Protocol.md).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
