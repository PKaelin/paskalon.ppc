# Alarm Lifecycle
How published data becomes a raised, acknowledged and normalized alarm and how the alarm actions are sent.

Back to the [Design Overview](./Overview.md).


## Flow
1. **Receive:** `PpcAlarmSubscriber` subscribes to the configured Redis topics and queues the received data in the `PpcAlarmMessageQueue`.
2. **Evaluate:** `PpcAlarmHandlerService` takes the queued data and the `PpcAlarmHandler` evaluates it with the `PpcAlarmEvaluator` against the enabled alarm configurations.
3. **Delay:** a value must stay abnormal for `TimeBeforeRaisingAlarmMilliseconds` before the alarm is raised and stay normal for `TimeBeforeNormalizingAlarmMilliseconds` before it is normalized. `PpcAlarmDelayService` raises and normalizes the delayed alarms once their delay elapsed (checked every `AlarmDelayCheckIntervalMilliseconds`).
4. **Persist:** raised and normalized alarms are written to the database and their alarm actions are stored in an outbox.
5. **Publish:** `AlarmActionOutboxPublisherService` sends the pending alarm actions to the RabbitMQ exchange every `AlarmActionSendIntervalMilliseconds`. A failed send is retried after `AlarmActionRetryIntervalMilliseconds`, so no action is lost while RabbitMQ is not reachable.
6. **Acknowledge:** operators acknowledge alarms over the Web API (`POST /api/v1/alarms/acknowledge`). An acknowledged alarm stays active until it is normalized.


## States

| From | Event | To |
|------|-------|----|
| – | Value abnormal for the raise delay | `Alarm` |
| `Alarm` | Acknowledged by an operator | `Acknowledged` |
| `Alarm` / `Acknowledged` | Value normal for the normalize delay | `Normal` |

The states are described in [Domain Model](../DomainModel.md#alarm-sequence-states).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
