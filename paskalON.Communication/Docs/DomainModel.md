# Communication Domain Model
Messages, heartbeats and dependency states of the Communication libraries.

Back to the [Overview](./Overview.md).


## Sequenced Messages
Every published message of a service implements `ISequencedMessage`:

| Property | Description |
|----------|-------------|
| `SourceInstance` | Identifies the publishing service instance (new per process start). |
| `Sequence` | Increasing sequence number per source instance. |
| `Timestamp` | Creation time of the message. |


## Heartbeat
`HeartbeatDto` is published periodically by a service to signal that it is alive. It carries the `ServiceName` (e.g. PowerControl) and the sequence stamp.


## Dependency State

| State | Description |
|-------|-------------|
| `Lost` | The dependency has not been seen within its loss timeout (also from startup until it is first seen). |
| `Available` | The dependency was seen within its loss timeout. |


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
