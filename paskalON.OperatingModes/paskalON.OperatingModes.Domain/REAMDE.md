# Operating Modes


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


## Types of operating modes


### Closed Modes (Closed-Loop Control)

| Mode | Purpose | Inputs (for calculating output) | Output Controlled | What Output Influences |
|------|---------|---------------------------------|-------------------|------------------------|
| Maintenance SOC Mode | Takes a unit out of available units and commands P and or Q setpoints | SOC |  Active Power (P) and or Reactive Power (Q) | Active and or Reactive Power |


#### Voltage / Reactive Power (Q) Control Modes

| Mode | Purpose | Inputs (for calculating output) | Output Controlled | What Output Influences |
|------|---------|---------------------------------|-------------------|------------------------|
| Reactive Power Mode | Fixed MVar setpoint | Measured Q at POI, Q setpoint, feedback error | Reactive Power (Q) | Voltage at POI |
| Voltage Mode | Regulate voltage at POI by adjusting Reactive Power | Measured voltage at POI, voltage setpoint, Q capability | Reactive Power Q | Grid voltage at POI |
| Voltage Droop Mode | Regulate voltage at POI by adjusting Reactive Power | Measured voltage at POI, voltage setpoint, droop/slope gain, Q capability | Reactive Power Q | Grid voltage at POI |
| Power Factor Mode | Maintain a target power factor at POI regardless of active power output | Measured P & Q at POI, PF setpoint | Reactive Power (Q) | Voltage (indirectly) compliance with PF requirements.
| Voltage Var Droop Mode | Curtail reactive power when voltage rises above a threshold | Measured voltage, volt-var curve/setpoints | Reactive Power (P) | Voltage at POI (limit overvoltage) |


#### Frequency / Active Power (P) Control Modes

| Mode | Purpose | Inputs (for calculating output) | Output Controlled | What Output Influences |
|------|---------|---------------------------------|-------------------|------------------------|
| Active Power Mode | Fixed Watt setpoint | Measured P at POI, P setpoint, feedback error |  Active Power (P) | Grid frequency, power balance |
| Maximum Active Power Limit Mode | Restricts the active power output to a cap | Measured P at POI, P Max setpoint |  Active Power (P) |  Active Power |
| Frequency Watt Mode | Automatically adjust active power in response to frequency deviations | Measured frequency, frequency reference value |  Active Power (P) | Grid frequency |
| Frequency Droop Mode | Automatically adjust active power in response to frequency deviations | Measured frequency, frequency reference value, droop settings, available reserves |  Active Power (P) | Grid frequency |


#### Voltage / Active Power (P) Control Modes

| Mode | Purpose | Inputs (for calculating output) | Output Controlled | What Output Influences |
|------|---------|---------------------------------|-------------------|------------------------|
| Voltage Watt Droop Mode | Curtail active power when voltage rises above a threshold | Measured voltage, volt-watt curve/setpoints | Active Power (P) | Voltage at POI (limit overvoltage) |


#### Energy Storage Specific Closed-Loop Modes

| Mode | Purpose | Inputs (for calculating output) | Output Controlled | What Output Influences |
|------|---------|---------------------------------|-------------------|------------------------|
| Charge Discharge Mode | Charge discharge batteries till limits are reached | SOC |  Active Power (P) | State Of Charge (SOC) |
| Coordinated Charge Discharge Mode | Charge discharge batteries to a specific SOC | SOC |  Active Power (P) | State Of Charge (SOC) |


### Open Mode (Open-Loop Control)

| Mode | Purpose | Inputs (for calculating output) | Output Controlled | What Output Influences |
|------|---------|---------------------------------|-------------------|------------------------|
| Active Power Fixed Mode | Set a fixed setpoint without feedback signal |  Active Power setpoint, Available P |  Active Power (P) |  Active Power |
| Reactive Power Fixed Mode | Set a fixed setpoint without feedback signal | Reactive Power setpoint, Available Q | Reactive Power (Q) | Reactive Power |
| Maintenance Mode | Takes a unit out of available units and commands P and or Q setpoints | Active Power setpoint, Reactive Power setpoint, Available P&Q |  Active Power (P) and or Reactive Power (Q) | Active and or Reactive Power |


#### Energy Resources Specific Open-Loop Modes

| Mode | Purpose | Inputs (for calculating output) | Output Controlled | What Output Influences |
|------|---------|---------------------------------|-------------------|------------------------|
Maximum Power Point Tracking Mode (MPPT) | Maximize energy yield by continuously adjusting the inverter's input electrical characteristics | Available active power (P-Available) |  Active Power (P) |  Active Power |


## Droop control
Droop control is a decentralized, proportional control mechanism that adjust the output proportionally in response to its input. 
Operating modes with droop control use a curve controller.

