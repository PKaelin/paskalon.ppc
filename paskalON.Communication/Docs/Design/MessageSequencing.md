# Message Sequencing
How receivers drop outdated and duplicate messages.

Back to the [Design Overview](./Overview.md).


## Concept
- A publisher stamps every message with a `MessageStamp` created by the `MessageSequencer`: its source instance, an increasing sequence number and the creation time.
- The source instance is new per process start, so the receivers do not drop the sequences of a restarted service.
- A receiver accepts a message with the `MessageOrderGuard` only if it is newer than the last accepted message of the same source instance. Outdated and duplicate messages are ignored.

The setpoints of the Operating Modes service and all heartbeats are sequenced messages.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
