# Operating Modes
This solution is to self contain all logic regarding the implementation of Operating Modes.
An Operating Mode defines the specific behavior and control strategy the system uses to interact with the power grid.  
*Note: paskalON.Common libraries are project references during initial development*  

## Types of operating modes


### Non-Metered Mode (Open-Loop Control)
- How it works: The controller sends targets and assumes the action happens perfectly.
- Feedback: None. It does not measure any actual output or adjust any changes.
- Power plant use: In predictable scenarios.

![Non-Metered Operating Mode Overview](./Non-Metered%20Operating%20Mode%20Overview.drawio.svg)


### Metered Mode (Closed-Loop Control)
- How it works: Consistently checks the output using signals and compares it to the target.
- Feedback: Continuous. If output drifts the controller calculates an error signal and makes adjustments in real time.
- Power plant use: Industry standard for safe and stable operation. Used in Automatic Generation Control (AGC) and Automatic Voltage Regulators (AVR) to ensure grid compliance and prevent outages.

![Metered Operating Mode Overview](./Metered%20Operating%20Mode%20Overview.drawio.svg)


## High level concept
Operating modes can be stacked. Operating modes can be defined as "Additive"(each layer calculates output and adds it to its previous output) or "Exclusive" (uses its output only).

![Operating Modes High Level Concept](./Operating%20Modes%20High%20Level%20Concept.drawio.svg)


## Ramp Model 
Every operating mode has a ramp model. The ramp model provides the ability to smoothly change power targets between set points.

![Ramp Models Overview](./Ramp%20Models%20Overview.drawio.svg)


## Curve Configuration
Every operating mode can have a curve configuration.

![Curve Overview](./Curve%20Configuration%20Overview.drawio.svg)

- If latest signal reading is below or above first/last configured then assume first/last (Input +50/-50 in chart are not configured but are virtually as they are beyond the first/last).
- If latest signal reading is between two IF values, then calculate the appropriate system output by a linear interpolation between the two corresponding THEN values.
- If two adjoining then values are both the same then this would be called a deadband area.
- If you want respond to ascending system readings with one set of conditions, and descending readings with a different set of conditions, you can configure a bi-directional curve.
- Curve changes a target according to a source but does not overrule the ramp model.
- Directional curve: all points going up or all down. Bi-Directional curve: points going up then down then up or points going down then up then down.




TODO: Add more information.
