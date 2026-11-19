# Header Events
The alarm shapes in the always-visible header of Luna Web.

Back to the [Design Overview](./Overview.md).


## Behavior
The header has four alarm shapes and they update live whenever an alarm action reaches Luna.

- The number in the shape is all active alarms of that severity, unacknowledged plus acknowledged. It drops as alarms return to Normal and above 99 it shows "99+".
- The tooltip shows the split. For example "Error: 5 active (2 unacknowledged, 3 acknowledged)".

The counts are pushed by the `AlarmHub`.


## Shapes And Colors
The shapes and colors of the severities are defined in [Colors](/Docs/03-Design/Colors.md#events-color--shapes).


## States
| State | Look |
|-------|------|
| Counts unknown (no counts yet or the browser lost its connection to Luna) | Gray outline; tooltip "alarm state unknown" |
| No active alarms | Outline (border) in the shape's color |
| Active alarms, at least one unacknowledged | Filled, shows the number, blinks |
| Active alarms, all acknowledged | Filled, shows the number, no blinking |


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
