# Metrics
Metrics published by the operating modes and the system operating mode control.

Back to the [Design Overview](./Overview.md).
How metrics are published in general is described in [Telemetry](/Docs/03-Design/Telemetry.md).


Every operating mode has its own metrics publisher tagged with `Name`, `Classification` and `DerUnit` (override modes only).
Axis metrics are only published for the power axes an operating mode has.

| Level | Metrics |
|-------|---------|
| Operating mode | `State`, `IsActivated` |
| Power control | `ActivePowerState`, `ActivePowerSetpoint`, `ActivePowerAvailable`, `ActivePowerTargetSetpoint`, `ActivePowerTarget` (same for `ReactivePower`) |
| Feedback control | `ActivePowerErrorAdjustment`, `ActivePowerCorrected`, `ActivePowerFeedbackReference`, `ActivePowerFeedbackMeasured`, `ActivePowerFeedbackError` (same for `ReactivePower`) |
| Contribution (stack modes) | `ActivePowerContribution`, `ReactivePowerContribution`, `ActivePowerIsLimiting`, `ReactivePowerIsLimiting` (limiting modes) |
| System | `State`, `SetpointActivePower`, `SetpointReactivePower`, `SetpointActivePowerActual`, `SetpointReactivePowerActual`, `ActivePowerObjective`, `ReactivePowerObjective`, `ActivePowerTransition`, `ReactivePowerTransition`, `ActivePowerTransitionOffset`, `ReactivePowerTransitionOffset`, `ActivePowerTransitionCount`, `ReactivePowerTransitionCount`, `BaseActivePower`, `BaseReactivePower`, `AdditiveActivePowerDelta`, `AdditiveReactivePowerDelta`, `IsActivePowerLimited`, `IsReactivePowerLimited`, `OverrideCount` |

The contribution is the change of the final value caused by an operating mode in the last cycle. The contributions of all
operating modes on an axis add up to the objective. The applied target differs from the objective by the transition offset.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
