# Operating Modes
This solution is to self contain all logic regarding the implementation of Operating Modes.
An Operating Mode defines the specific behavior and control strategy the system uses to interact with the power grid.  
*Note: paskalON.Common libraries are project references during initial development*  


## Operating Mode Overview
![Operating Mode Overview](./Docs/Operating%20Mode%20Overview.drawio.svg)


## Structure

### Project .Domain.Configs
Persistent Domain Configuration layer of DDD described in [README.md](./paskalON.OperatingModes.Domain.Configs/README.md)

### Project .Domain
Domain layer of DDD described in [README.md](./paskalON.OperatingModes.Domain/README.md)

### Project .Infrastructure
Infrastructure layer of DDD described in [README.md](./paskalON.OperatingModes.Infrastructure/README.md)

---

TODO: Add general add readmes for not yet created projects below.

### Project .Application
Application layer of DDD described in [README.md](./paskalON.OperatingModes.Application/README.md)


### Project .Service
Service/Interface layer described in [README.md](./paskalON.OperatingModes.Service/README.md)

---


## Types of operating modes


### Open Mode (Open-Loop Control)
- How it works: The controller sends targets and assumes the action happens perfectly.
- Feedback: None. It does not measure any actual output or adjust any changes.
- Power plant use: In predictable scenarios.

![Non-Metered Operating Mode Overview](./Docs/Non-Metered%20Operating%20Mode%20Overview.drawio.svg)
 

### Closed Mode (Closed-Loop Control)
- How it works: Consistently checks the output using signals and compares it to the target.
- Feedback: Continuous. If output drifts the controller calculates an error signal and makes adjustments in real time.
- Power plant use: Industry standard for safe and stable operation. Used in Automatic Generation Control (AGC) and Automatic Voltage Regulators (AVR) to ensure grid compliance and prevent outages.

![Metered Operating Mode Overview](./Docs/Metered%20Operating%20Mode%20Overview.drawio.svg)


## High level concept
Operating modes can be stacked. Operating modes can be defined as "Additive"(each layer calculates output and adds it to its previous output) or "Exclusive" (uses its output only).

![Operating Modes High Level Concept](./Docs/Operating%20Modes%20High%20Level%20Concept.drawio.svg)


## Ramp Model 
Every operating mode has a ramp model. The ramp model provides the ability to smoothly change power targets between set points.

![Ramp Models Overview](./Docs/Ramp%20Models%20Overview.drawio.svg)


## Curve Configuration
Every operating mode can have a curve configuration.

![Curve Overview](./Docs/Curve%20Configuration%20Overview.drawio.svg)

- If latest signal reading is below or above first/last configured then assume first/last (Input +50/-50 in chart are not configured but are virtually as they are beyond the first/last).
- If latest signal reading is between two IF values, then calculate the appropriate system output by a linear interpolation between the two corresponding THEN values.
- If two adjoining then values are both the same then this would be called a deadband area.
- If you want respond to ascending system readings with one set of conditions, and descending readings with a different set of conditions, you can configure a bi-directional curve.
- Curve changes a target according to a source but does not overrule the ramp model.
- Directional curve: all points going up or all down. Bi-Directional curve: points going up then down then up or points going down then up then down.


## More information
[See project readmes in structure section](./README.md#structure)