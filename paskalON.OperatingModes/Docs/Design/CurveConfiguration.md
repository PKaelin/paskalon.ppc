# Curve Configuration
Every operating mode can have a curve configuration.

Back to the [Design Overview](./Overview.md).

![Curve Overview](../Assets/Diagrams/Curve%20Configuration%20Overview.drawio.svg)


## Rules
- If latest signal reading is below or above first/last configured then assume first/last (Input +50/-50 in chart are not configured but are virtually as they are beyond the first/last).
- If latest signal reading is between two IF values, then calculate the appropriate system output by a linear interpolation between the two corresponding THEN values.
- If two adjoining then values are both the same then this would be called a deadband area.
- If you want respond to ascending system readings with one set of conditions, and descending readings with a different set of conditions, you can configure a bi-directional curve.
- Curve changes a target according to a source but does not overrule the ramp model.
- Directional curve: all points going up or all down. Bi-Directional curve: points going up then down then up or points going down then up then down.


## Curve Types
The curve configurations in `paskalON.OperatingModes.Domain.Configs`:
- `VoltVarCurveConfig`, `VoltWattCurveConfig`, `FrequencyWattCurveConfig`.
- Voltage Fault Ride-Through (VFRT) curves: `VfrtHighMomentaryCessationCurveConfig`, `VfrtHighTripCurveConfig`, `VfrtLowMomentaryCessationCurveConfig`, `VfrtLowTripCurveConfig`.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
