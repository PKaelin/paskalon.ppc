# Luna (User Interface)


## Header events
The header has four alarm shapes and they update live whenever an alarm action reaches Luna.

- The number in the shape is all active alarms of that severity, unacknowledged plus acknowledged. It drops as alarms return to Normal and above 99 it shows "99+".
- The tooltip shows the split. For example "Error: 5 active (2 unacknowledged, 3 acknowledged)".


### Header event shapes & colors & states

| Event Type | Shape | Color |
|------------|-------|-------|
| Critical | Octagon (stop sign) | #B91C1C |
| Error | Circle | #EF4444 |
| Warning | Triangle | #FFDB44 |
| Info | Square | #06B6D4 |


| State | Look |
|-------|------|
| Counts unknown (no counts yet or the browser lost its connection to Luna) | Gray outline; tooltip "alarm state unknown" |
| No active alarms | Outline (border) in the shape's color |
| Active alarms, at least one unacknowledged | Filled, shows the number, blinks |
| Active alarms, all acknowledged | Filled, shows the number, no blinking |

