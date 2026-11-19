# Transitions
How the applied targets follow the objectives when operating modes are added, removed, replaced or change their target.

Back to the [Design Overview](./Overview.md).


## Power control definitions
- Setpoint: Value the operating mode aims for (supervisory setpoint, operator setpoint or available power).
- TargetSetpoint: Setpoint reduced to the magnitude of the available power and limited by the configured limits. End point of the ramp.
- Target: Output of the power axis.

There is one ramp layer for stack operating modes: their power control sets the target to the target setpoint immediately,
also when the setpoint or the available power changes outside its deadband. The `SystemOperatingModeControl` transitions
the applied target with the ramp of the operating mode (see [Transition segments](#transition-segments)), so `ActivePowerObjective` shows
the new target immediately and `SetpointActivePowerActual` ramps. Override operating modes ramp their target in the power
control because their targets are applied directly to a DER unit. When a plant-wide override (no target DER unit) is
activated, the `SystemOperatingModeControl` seeds its power controls with the current applied targets (`SetStartTarget`),
so its ramp starts where the plant is, e.g. the `ShutdownMode` ramps the applied active and reactive power to the configured
`ShutdownTargetActivePowerWatts` and `ShutdownTargetReactivePowerVars` (zero by default) and
reports `IsShutdownCompleted` once both axes reached their targets.


## Transition segments

The objective is the target calculated from the stack. The applied target is the published target. A transition of a power
axis consists of one segment per changed operating mode. Each segment ramps the step of its operating mode to zero with the
ramp of that operating mode:

`applied = objective + Σ segment offsets`

- Step of an operating mode: its contribution before the change minus its current contribution. A removed mode drops its
  last contribution, an added mode ramps in from zero, a replacement is one segment (contribution of the predecessor minus
  contribution of the replacing mode) with the ramp of the replacing mode.
- Bumpless: at a change the new segments together cover `origin - objective - offsets of the previous cycle`. The part not
  explained by the steps of the changed modes (e.g. a limiting mode reacting to a replaced base mode) is assigned to the
  segment of the operating mode owning the axis, otherwise to the largest segment. A base replacement under a binding
  limit therefore starts no transition.
- Several changes in the same cycle (e.g. two removed operating modes) start one segment each with their own ramp.
  A change while other segments are running adds a segment, the running segments keep their own progress and ramp.
  A further change of the same operating mode is merged into its running segment.
- Dynamic contributions (e.g. additive corrections) pass through, only the steps caused by the changes are ramped. A limit
  applies to the objective immediately while the segments determine the command trajectory.
- A new segment starts in the cycle of the change with its full step, so the applied target equals the origin at the change.

| Kind | Trigger | Origin | Ramp |
|------|---------|--------|------|
| `InitialActivation` | Operating modes present before the first cycle | Measured power at the POI if valid (reported and within the nameplate), otherwise zero | Each present mode (own segment) |
| `ModeAdded` | Operating mode added while running | Current applied target | Added mode |
| `ModeReplacement` | Base / exclusive mode replaced | Current applied target (base portion `BaseActivePower` / `BaseReactivePower` plus unchanged contributions) | Replacing mode |
| `ModeRemoved` | Operating mode removed while running | Current applied target | Removed mode |
| `TargetChanged` | Target setpoint of a stack operating mode changed (setpoint or available power) | Current applied target | Changed mode (merged into its running segment) |

| Classification | Added while running | Removed while running |
|----------------|---------------------|-----------------------|
| Base / Exclusive | Transfer control from the applied target to the new objective | Transition to the objective of a replacement, without replacement to zero |
| Additive | Transition from zero correction to the target including the delta | Transition to the target without the delta |
| Limiting | Transition to the limited target | Transition to the target without the limit, remaining constraints still apply |

The state of an operating mode follows its segment: an added or replacing operating mode is `RampingToEnabled` until the
applied target reached its target, a removed operating mode is `RampingToDisabled` until its contribution is ramped to
zero, then `Enabled` or `Disabled`.

An operating mode waiting for available power starts its segment once it established its target. Mandatory constraints
(nameplate and plant-wide overrides) are never delayed by a transition: a constraint in the direction of the running
offset scales the segments down, otherwise the segments are cancelled. A plant-wide override cancels the segments.



## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
