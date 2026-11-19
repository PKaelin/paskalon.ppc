# Alarms Domain Model
Alarms, severities and the alarm sequence of the Alarms solution.

Back to the [Overview](./Overview.md).


## Alarms
- `AlarmBase`: base of every alarm with its configuration and its state in the alarm sequence.
- `PpcAlarm`: alarm raised from the data published by a PPC service or device, identified by its alarm type and device.
- `PpcAlarmEvaluator`: evaluates the received values against the alarm configurations and returns the pending raise and normalize transitions.


## Alarm Types
The `PpcAlarmType` is the type of the publishing device or service:
`SystemAlarm`, `PcsAlarm`, `BatteryAlarm`, `BatteryContainerAlarm`, `SolarPanelAlarm`, `GenericModbusDeviceAlarm`, `SystemPowerMeterAlarm`, `CircuitPowerMeterAlarm`, `ExternalPowerMeterAlarm`, `AuxiliaryPowerMeterAlarm`, `PowerControlAlarm`, `OperatingModeAlarm`.


## Severity Levels

| Severity | Value |
|----------|-------|
| Info | 0 |
| Warning | 1 |
| Error | 2 |
| Critical | 3 |

The colors and shapes of the severities in the user interfaces are defined in [Colors](/Docs/03-Design/Colors.md#events-color--shapes).


## Alarm Sequence States

| State | Description |
|-------|-------------|
| `Alarm` | The alarm is active and has not been acknowledged or reset. |
| `Acknowledged` | The alarm is active and has been acknowledged but has not been reset. |
| `Normal` | The alarm has been reset and is not active. |

How an alarm moves through these states is described in [Alarm Lifecycle](./Design/AlarmLifecycle.md).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
