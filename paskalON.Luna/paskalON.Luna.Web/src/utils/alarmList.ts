import {
    AlarmSequenceState,
    AlarmSeverity,
    type PpcAlarmDto,
    type PpcAlarmFilter
} from '../types/alarm';

/** A choice of a filter select, value null means all. */
export interface FilterOption<T> {
    label: string;
    value: T;
}

export const severityLabels: Record<AlarmSeverity, string> = {
    [AlarmSeverity.Info]: 'Info',
    [AlarmSeverity.Warning]: 'Warning',
    [AlarmSeverity.Error]: 'Error',
    [AlarmSeverity.Critical]: 'Critical'
};

export const stateLabels: Record<AlarmSequenceState, string> = {
    [AlarmSequenceState.Alarm]: 'Alarm',
    [AlarmSequenceState.Acknowledged]: 'Acknowledged',
    [AlarmSequenceState.Normal]: 'Normal'
};

/** CSS modifier of a severity, the colours are the ones of the alarm shapes in the header. */
export const severityClasses: Record<AlarmSeverity, string> = {
    [AlarmSeverity.Info]: 'alarm-severity-info',
    [AlarmSeverity.Warning]: 'alarm-severity-warning',
    [AlarmSeverity.Error]: 'alarm-severity-error',
    [AlarmSeverity.Critical]: 'alarm-severity-critical'
};

export const severityOptions: FilterOption<AlarmSeverity | null>[] = [
    { label: 'All', value: null },
    { label: 'Info', value: AlarmSeverity.Info },
    { label: 'Warning', value: AlarmSeverity.Warning },
    { label: 'Error', value: AlarmSeverity.Error },
    { label: 'Critical', value: AlarmSeverity.Critical }
];

export const triggeredOptions: FilterOption<number>[] = [
    { label: 'Past 8 hours', value: 8 },
    { label: 'Past 24 hours', value: 24 },
    { label: 'Past 7 days', value: 7 * 24 },
    { label: 'Past 30 days', value: 30 * 24 }
];

export const stateOptions: FilterOption<AlarmSequenceState | null>[] = [
    { label: 'All', value: null },
    { label: 'Alarm', value: AlarmSequenceState.Alarm },
    { label: 'Acknowledged', value: AlarmSequenceState.Acknowledged },
    { label: 'Normal', value: AlarmSequenceState.Normal }
];

/** The filter after "Clear All": all alarms of the past 8 hours. */
export function defaultAlarmFilter(): PpcAlarmFilter {
    return {
        triggeredWithinHours: 8,
        severityLevel: null,
        alarmSequenceState: null,
        deviceName: ''
    };
}

const dateTimeFormat = new Intl.DateTimeFormat(undefined, {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
});

/** Local date and time of an ISO 8601 timestamp. */
export function formatAlarmTime(isoTimestamp: string): string {
    const time = Date.parse(isoTimestamp);

    return Number.isNaN(time) ? isoTimestamp : dateTimeFormat.format(time);
}

/**
 * Time of the acknowledgement of an alarm, '—' while it can still be acknowledged,
 * 'Not acknowledgeable' for a configuration that cannot be acknowledged. Who acknowledged it has its own column.
 */
export function formatAcknowledgement(alarm: PpcAlarmDto): string {
    if (alarm.acknowledgedOn !== null) {
        return formatAlarmTime(alarm.acknowledgedOn);
    }

    return alarm.ppcAlarmConfig.isAcknowledgeable ? '—' : 'Not acknowledgeable';
}

/** Time of the acknowledgement for sorting, alarms that are not acknowledged sort as the oldest. */
function acknowledgedTime(alarm: PpcAlarmDto): number {
    return alarm.acknowledgedOn === null ? Number.NEGATIVE_INFINITY : Date.parse(alarm.acknowledgedOn);
}

/** Columns the alarm list can be sorted by. */
export type AlarmSortKey = 'severity' | 'triggered' | 'acknowledged' | 'state' | 'device' | 'lastChange';

export type SortDirection = 'asc' | 'desc';

export interface AlarmSort {
    key: AlarmSortKey;
    direction: SortDirection;
}

/** Default order: the latest triggered alarm first. */
export const defaultAlarmSort: AlarmSort = { key: 'triggered', direction: 'desc' };

function compareKey(left: PpcAlarmDto, right: PpcAlarmDto, key: AlarmSortKey): number {
    switch (key) {
        case 'severity':
            return left.ppcAlarmConfig.severityLevel - right.ppcAlarmConfig.severityLevel;
        case 'triggered':
            return Date.parse(left.createdOn) - Date.parse(right.createdOn);
        case 'acknowledged':
            return acknowledgedTime(left) - acknowledgedTime(right);
        case 'state':
            return left.alarmSequenceState - right.alarmSequenceState;
        case 'device':
            return (left.deviceName ?? '').localeCompare(right.deviceName ?? '', undefined, { numeric: true, sensitivity: 'base' });
        case 'lastChange':
            return Date.parse(left.changedDate) - Date.parse(right.changedDate);
    }
}

/**
 * The alarms sorted by the column. Alarms with the same value keep the default order (the latest triggered first),
 * so the order is stable after every reload.
 */
export function sortAlarms(alarms: PpcAlarmDto[], sort: AlarmSort): PpcAlarmDto[] {
    const sign = sort.direction === 'asc' ? 1 : -1;

    return [...alarms].sort((left, right) => (
        sign * compareKey(left, right, sort.key)
        || compareKey(right, left, 'triggered')
        || right.id - left.id
    ));
}
