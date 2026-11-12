import { ref, type Ref } from 'vue';
import { defaultAlarmFilter } from '../utils/alarmList';
import type { AlarmSeverity, PpcAlarmFilter } from '../types/alarm';

/** Filter of the alarm list. Shared by the whole app, so it is kept while the tab is switched. */
const filter = ref<PpcAlarmFilter>(defaultAlarmFilter());

/**
 * The filter of the alarm list. Every change replaces the filter object, so that a watcher of the
 * Alarms view reloads the alarms.
 */
export function useAlarmFilter(): Ref<PpcAlarmFilter> {
    return filter;
}

/**
 * Filters the alarm list only by the severity, e.g. after a click on an event shape. The other criteria are
 * reset to their defaults, so that the list shows the same alarms the shape counts.
 */
export function filterAlarmsBySeverity(severityLevel: AlarmSeverity): void {
    filter.value = { ...defaultAlarmFilter(), severityLevel };
}
