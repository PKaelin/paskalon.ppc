# Operating Modes Domain


## Categories and structure of operating modes

```
OperatingModes
|
|___OpenModes
|   |___VoltageReactives
|   |   |___Open-Loop voltage and reactive power control modes.
|   |
|   |___FrequencyActives
|   |   |___Open-Loop frequency and active power control modes.
|   |
|   |___EnergyResources 
|       |___Open-Loop energy resource modes.
|
|___ClosedModes
    |___VoltageReactives
    |   |___Closed-Loop voltage and reactive power control modes.
    |
    |___FrequencyActives
    |   |___Closed-Loop frequency and active power control modes.
    |
    |___EnergyStorages
        |___Closed-Loop energy storage control modes.
```

## Classifications of operating modes

| Classification | Position | Cardinality | Description |
|----------------|----------|-------------|-------------|
| Override | Before the stack | 0..1 per unit | Force P or Q targets and remove a unit from normal dispatch. |
| Exclusive Active | First or second in the stack (mutually ordered with Exclusive Reactive) | 0..1 per active power type | The only active power mode that can be active at a time. |
| Exclusive Reactive | First or second in the stack (mutually ordered with Exclusive Active) | 0..1 per reactive power type | The only reactive power mode that can be active at a time. |
| Base Active | First or second in the stack (mutually ordered with Base Reactive) | Exactly one per active power type | The primary active power mode that determines the target. |
| Base Reactive | First or second in the stack (mutually ordered with Base active) | Exactly one per reactive power type | The primary reactive power mode that determines the target. |
| Additive Active | After the base must have a Base Active before this mode | 0..n | Contributes a correction or offset that can be combined with another mode's output. |
| Additive Reactive | After the base must have a Base Reactive before this mode | 0..n | Contributes a correction or offset that can be combined with another mode's output. |
| Limiting Active | After the additives must have a Base Active before this mode| 0..n | Imposes a boundary on the combined target. It does not necessarily replace the underlying control mode. |
| Limiting Reactive | After the additives must have a Base Reactive before this mode | 0..n | Imposes a boundary on the combined target. It does not necessarily replace the underlying control mode. |
 


## Types of operating modes


### Closed Modes (Closed-Loop Control)

| Mode | Purpose | Inputs (for calculating output) | Output Controlled | What Output Influences | P Classification | Q Classification |
|------|---------|---------------------------------|-------------------|------------------------|------------------|------------------|
| Maintenance SOC Mode | Takes a unit out of available units and commands P and or Q setpoints | SOC |  Active Power (P) and or Reactive Power (Q) | Active and or Reactive Power | Override | Override |


#### Voltage / Reactive Power (Q) Control Modes

| Mode | Purpose | Inputs (for calculating output) | Output Controlled | What Output Influences | P Classification | Q Classification |
|------|---------|---------------------------------|-------------------|------------------------|------------------|------------------|
| Reactive Power Mode | MVar setpoint | Measured Q at POI, Q setpoint, feedback error | Reactive Power (Q) | Voltage at POI | - | Base Reactive |
| Voltage Mode | Regulate voltage at POI by adjusting Reactive Power | Measured voltage at POI, voltage setpoint, Q capability | Reactive Power Q | Grid voltage at POI | - | Base Reactive |
| Voltage Droop Mode | Regulate voltage at POI by adjusting Reactive Power | Measured voltage at POI, voltage setpoint, droop/slope gain, Q capability | Reactive Power Q | Grid voltage at POI | - | Base Reactive |
| Power Factor Mode | Maintain a target power factor at POI regardless of active power output | Measured P & Q at POI, PF setpoint | Reactive Power (Q) | Voltage (indirectly) compliance with PF requirements.  - | Base Reactive (reads P) |
| Voltage Var Droop Mode | Curtail reactive power when voltage rises above a threshold | Measured voltage, volt-var curve/setpoints | Reactive Power (P) | Voltage at POI (limit overvoltage) |  - | Limiting Reactive |


#### Frequency / Active Power (P) Control Modes

| Mode | Purpose | Inputs (for calculating output) | Output Controlled | What Output Influences | P Classification | Q Classification |
|------|---------|---------------------------------|-------------------|------------------------|------------------|------------------|
| Active Power Mode | Watt setpoint | Measured P at POI, P setpoint, feedback error |  Active Power (P) | Grid frequency, power balance | Base Active | - |
| Maximum Active Power Limit Mode | Restricts the active power output to a cap | Measured P at POI, P Max setpoint |  Active Power (P) |  Active Power | Limiting Active | - |
| Frequency Watt Mode | Automatically adjust active power in response to frequency deviations | Measured frequency, frequency reference value |  Active Power (P) | Grid frequency | Additive Active | - |
| Frequency Droop Mode | Automatically adjust active power in response to frequency deviations | Measured frequency, frequency reference value, droop settings, available reserves |  Active Power (P) | Grid frequency | Additive Active | - |


#### Voltage / Active Power (P) Control Modes

| Mode | Purpose | Inputs (for calculating output) | Output Controlled | What Output Influences | P Classification | Q Classification |
|------|---------|---------------------------------|-------------------|------------------------|------------------|------------------|
| Voltage Watt Droop Mode | Curtail active power when voltage rises above a threshold | Measured voltage, volt-watt curve/setpoints | Active Power (P) | Voltage at POI (limit overvoltage) | Limiting Active | - |


#### Energy Storage Specific Closed-Loop Modes

| Mode | Purpose | Inputs (for calculating output) | Output Controlled | What Output Influences | P Classification | Q Classification |
|------|---------|---------------------------------|-------------------|------------------------|------------------|------------------|
| Charge Discharge Mode | Charge discharge batteries till limits are reached | SOC |  Active Power (P) | State Of Charge (SOC) | Base Active | - |
| Coordinated Charge Discharge Mode | Charge discharge batteries to a specific SOC | SOC |  Active Power (P) | State Of Charge (SOC) | Base Active | - |


### Open Mode (Open-Loop Control)

| Mode | Purpose | Inputs (for calculating output) | Output Controlled | What Output Influences | P Classification | Q Classification |
|------|---------|---------------------------------|-------------------|------------------------|------------------|------------------|
| Active Power Fixed Mode | Set a fixed setpoint without feedback signal |  Active Power setpoint, Available P |  Active Power (P) |  Active Power | Exclusive Active | - |
| Reactive Power Fixed Mode | Set a fixed setpoint without feedback signal | Reactive Power setpoint, Available Q | Reactive Power (Q) | Reactive Power | - | Exclusive Reactive |
| Maintenance Mode | Takes a unit out of available units and commands P and or Q setpoints | Active Power setpoint, Reactive Power setpoint, Available P&Q |  Active Power (P) and or Reactive Power (Q) | Active and or Reactive Power |  Override | Override |


#### Energy Resources Specific Open-Loop Modes

| Mode | Purpose | Inputs (for calculating output) | Output Controlled | What Output Influences | P Classification | Q Classification |
|------|---------|---------------------------------|-------------------|------------------------|------------------|------------------|
| Maximum Power Point Tracking Mode (MPPT) | Maximize energy yield by continuously adjusting the inverter's input electrical characteristics | Available active power (P-Available) |  Active Power (P) |  Active Power | Base Active | - |


## Droop control
Droop control is a decentralized, proportional control mechanism that adjust the output proportionally in response to its input. 
Operating modes with droop control use a curve controller.


## Design of the operating modes

Each concern is expressed with one mechanism:

| Concern | Mechanism | Types |
|---------|-----------|-------|
| Loop type | Inheritance | `OperatingModeBase` → `OperatingOpenModeBase`, `OperatingClosedModeBase` |
| Power axis | Composition | `PowerControl<ActivePower>` (`ActiveControl`), `PowerControl<ReactivePower>` (`ReactiveControl`) |
| Closed loop correction | Composition | `FeedbackControl<TPower>` (`ActiveFeedback`, `ReactiveFeedback`) |
| Classification | Interfaces | `IOverrideMode`, `IExclusiveActiveMode`, `IBaseActiveMode`, `IAdditiveActiveMode`, `ILimitingActiveMode`, ... |

The power controls are created from the classification interfaces: an operating mode with an active classification gets an
`ActiveControl`, one with a reactive classification gets a `ReactiveControl` and override modes get both.
Closed loop modes that correct their target with a feedback signal create a feedback control in their constructor with
`CreateActiveFeedback`/`CreateReactiveFeedback`. The measured value is read from the operating mode map (`*Map`), which is
filled with the device data by the `OperatingModeMapFactory`.

### Power control definitions
- Setpoint: Value the operating mode aims for (supervisory setpoint, operator setpoint or available power).
- TargetSetpoint: Setpoint reduced to the magnitude of the available power and limited by the configured limits. End point of the ramp.
- Target: Output of the power axis.

There is one ramp layer for stack operating modes: their power control sets the target to the target setpoint immediately,
also when the setpoint or the available power changes outside its deadband. The `SystemOperatingModeControl` transitions
the applied target with the ramp of the operating mode (see [Transitions](#transitions)), so `ActivePowerObjective` shows
the new target immediately and `SetpointActivePowerActual` ramps. Override operating modes ramp their target in the power
control because their targets are applied directly to a DER unit.


## Selection and lifecycle

| Flag / state | Meaning |
|--------------|---------|
| `IsActive` (configuration) | The configuration is loaded and available for selection. |
| `IsActivated` | The operating mode is committed to the stack or the override slot and activated by the system operating mode control. |

`OperatingModeStack` and `OperatingModeOverrides` validate every change before it is committed (`Add`, `Remove`, `Replace`,
`Move`) and record the committed changes. The `SystemOperatingModeControl` applies them atomically in its next calculation
cycle: added operating modes are activated, removed or replaced operating modes are deactivated immediately. A base mode
can not be removed while its dependent additive or limiting modes remain. `Replace` swaps a base or exclusive mode in one
atomic change at the same priority.


## Calculation cycle

`SystemOperatingModeControl` receives the supervisory setpoints with `UpdateSetpoints` and calculates periodically with `Calculate`:

1. Take a consistent snapshot of the committed stack, the override slot and their changes since the last cycle.
2. Activate added and deactivate removed or replaced operating modes.
3. Calculate the override modes (separate override slot, 0..1 per DER unit). Their targets are published as `OverrideSetpoints`.
4. Recalculate the entire stack in priority order. The final values of the `OperatingModeSignals` are the objectives.
5. Start a transition per affected power axis (see [Transitions](#transitions)).
6. Enforce the mandatory constraints immediately (plant-wide overrides and nameplate) and publish the applied targets
   (`SetpointActivePowerActual`, `SetpointReactivePowerActual`).

| Classification | Reads | Writes |
|----------------|-------|--------|
| Exclusive / Base | `SetpointActivePower` / `SetpointReactivePower` | `BaseActivePower` / `BaseReactivePower` |
| Additive | - | `AdditiveActivePowerDelta` / `AdditiveReactivePowerDelta` (+=) |
| Limiting | `FinalActivePower` / `FinalReactivePower` | `LimitedActivePower` / `LimitedReactivePower` |
| Override | - | Nothing, the targets are read from the power controls |

Overridden DER units are excluded from the plant-level available power.


## Transitions

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


## Metrics

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
