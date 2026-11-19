# Ramp Model
Every operating mode has a ramp model. The ramp model provides the ability to smoothly change power targets between set points.

Back to the [Design Overview](./Overview.md).

![Ramp Models Overview](../Assets/Diagrams/Ramp%20Models%20Overview.drawio.svg)


## Ramp Types
The ramp of an operating mode is configured with one of the ramp configurations in `paskalON.OperatingModes.Domain.Configs`:
- `RampRateConfig`: ramp rate.
- `RampRatePercentageConfig`: ramp rate as percentage.
- `RampTimeConfig`: ramp time.
- `RampTimeConstantConfig`: ramp time constant.

How the ramps are applied when operating modes change is described in [Transitions](./Transitions.md).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
