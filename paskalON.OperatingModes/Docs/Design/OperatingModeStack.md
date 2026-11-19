# Operating Mode Stack
How operating modes are stacked, selected and calculated by the system operating mode control.

Back to the [Design Overview](./Overview.md).


## High level concept
- Operating modes can be stacked. 
- Operating modes can be defined as "Additive"(each layer calculates output and adds it to its previous output) or "Exclusive" (uses its output only).
- The classifications and their position in the stack are defined in the [Domain Model](../DomainModel.md#classifications-of-operating-modes).

![Operating Modes High Level Concept](../Assets/Diagrams/Operating%20Modes%20High%20Level%20Concept.drawio.svg)


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
5. Start a transition per affected power axis (see [Transitions](./Transitions.md)).
6. Enforce the mandatory constraints immediately (plant-wide overrides and nameplate) and publish the applied targets
   (`SetpointActivePowerActual`, `SetpointReactivePowerActual`).

| Classification | Reads | Writes |
|----------------|-------|--------|
| Exclusive / Base | `SetpointActivePower` / `SetpointReactivePower` | `BaseActivePower` / `BaseReactivePower` |
| Additive | - | `AdditiveActivePowerDelta` / `AdditiveReactivePowerDelta` (+=) |
| Limiting | `FinalActivePower` / `FinalReactivePower` | `LimitedActivePower` / `LimitedReactivePower` |
| Override | - | Nothing, the targets are read from the power controls |

Overridden DER units are excluded from the plant-level available power.



## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
