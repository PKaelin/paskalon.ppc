# Operating Modes Domain Model
Categories, classifications and types of the operating modes and how they are modelled in code.

Back to the [Overview](./Overview.md).


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
| Reactive Power Limit Mode | Restricts the reactive power output to a band between a minimum and a maximum | Measured Q at POI, Q Min and Q Max limits | Reactive Power (Q) | Reactive Power | - | Limiting Reactive |


#### Frequency / Active Power (P) Control Modes

| Mode | Purpose | Inputs (for calculating output) | Output Controlled | What Output Influences | P Classification | Q Classification |
|------|---------|---------------------------------|-------------------|------------------------|------------------|------------------|
| Active Power Mode | Watt setpoint | Measured P at POI, P setpoint, feedback error |  Active Power (P) | Grid frequency, power balance | Base Active | - |
| Active Power Limit Mode | Restricts the active power output to a band between a minimum and a maximum | Measured P at POI, P Min and P Max limits |  Active Power (P) |  Active Power | Limiting Active | - |
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


How the operating modes are combined and calculated is described in [Design](./Design/Overview.md).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
