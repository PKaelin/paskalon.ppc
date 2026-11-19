# Control Loops
Open-loop and closed-loop operating modes and droop control.

Back to the [Design Overview](./Overview.md).


## Open Mode (Open-Loop Control)
- How it works: The controller sends targets and assumes the action happens perfectly.
- Feedback: None. It does not measure any actual output or adjust any changes.
- Power plant use: In predictable scenarios.

![Non-Metered Operating Mode Overview](../Assets/Diagrams/Non-Metered%20Operating%20Mode%20Overview.drawio.svg)


## Closed Mode (Closed-Loop Control)
- How it works: Consistently checks the output using signals and compares it to the target.
- Feedback: Continuous. If output drifts the controller calculates an error signal and makes adjustments in real time.
- Power plant use: Industry standard for safe and stable operation. Used in Automatic Generation Control (AGC) and Automatic Voltage Regulators (AVR) to ensure grid compliance and prevent outages.

![Metered Operating Mode Overview](../Assets/Diagrams/Metered%20Operating%20Mode%20Overview.drawio.svg)


## Droop Control
Droop control is a decentralized, proportional control mechanism that adjust the output proportionally in response to its input. 
Operating modes with droop control use a curve controller, see [Curve Configuration](./CurveConfiguration.md).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
