# Operating Modes Design
The important functional and technical designs of the Operating Modes solution.

Back to the [Overview](../Overview.md).


## Designs
- [Control Loops](./ControlLoops.md): open-loop and closed-loop operating modes and droop control.
- [Operating Mode Stack](./OperatingModeStack.md): stacking, selection, lifecycle and calculation cycle.
- [Transitions](./Transitions.md): power control definitions and how the applied targets follow the objectives.
- [Ramp Model](./RampModel.md): smooth change of the power targets.
- [Curve Configuration](./CurveConfiguration.md): curve-based targets (e.g. volt-var, frequency-watt).
- [Metrics](./Metrics.md): metrics of the operating modes and the system control.

Designs that span multiple solutions, e.g. the dependency health of the Operating Modes service, are in [Fault Tolerance & Resilience](/Docs/03-Design/FaultTolerance.md).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
