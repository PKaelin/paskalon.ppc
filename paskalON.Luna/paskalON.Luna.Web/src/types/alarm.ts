/**
 * Number of active alarms per severity level pushed by the alarm hub (paskalON.Alarms.Dto AlarmSummaryDto).
 *
 * The counts without prefix are the unacknowledged alarms (sequence state Alarm), the acknowledged counts
 * are the alarms that were acknowledged but are not normal yet (sequence state Acknowledged).
 */
export interface AlarmSummaryDto {
    /** Time the alarm service calculated the summary (ISO 8601, UTC). */
    calculatedOn: string;
    infoCount: number;
    warningCount: number;
    errorCount: number;
    criticalCount: number;
    acknowledgedInfoCount: number;
    acknowledgedWarningCount: number;
    acknowledgedErrorCount: number;
    acknowledgedCriticalCount: number;
}

/** Severity level of an alarm (paskalON.Alarms.Dto AlarmSeverityDto), serialized as number. */
export const AlarmSeverity = {
    Info: 0,
    Warning: 1,
    Error: 2,
    Critical: 3
} as const;
export type AlarmSeverity = (typeof AlarmSeverity)[keyof typeof AlarmSeverity];

/** Condition of the process that caused the alarm (paskalON.Alarms.Dto AlarmProcessConditionDto), serialized as number. */
export const AlarmProcessCondition = {
    Normal: 0,
    Abnormal: 1
} as const;
export type AlarmProcessCondition = (typeof AlarmProcessCondition)[keyof typeof AlarmProcessCondition];

/** State of the alarm in the alarm sequence (paskalON.Alarms.Dto AlarmSequenceStateDto), serialized as number. */
export const AlarmSequenceState = {
    Alarm: 0,
    Acknowledged: 1,
    Normal: 2
} as const;
export type AlarmSequenceState = (typeof AlarmSequenceState)[keyof typeof AlarmSequenceState];

/**
 * Configuration of a PPC alarm (paskalON.Alarms.Dto PpcAlarmConfigDto).
 *
 * Name, description, possible causes, response and consequences are string identifiers
 * (e.g. {{PCS.OverTemp.Name}}) that are mapped to a language dependent text, see utils/alarmTexts.ts.
 */
export interface PpcAlarmConfigDto {
    alarmId: string;
    name: string;
    severityLevel: AlarmSeverity;
    description: string;
    possibleCauses: string | null;
    response: string | null;
    consequences: string | null;
}

/** PPC alarm of the alarm list (paskalON.Alarms.Dto PpcAlarmDto). */
export interface PpcAlarmDto {
    id: number;
    ppcAlarmType: number;
    /** Null for an alarm that is not a device alarm, e.g. a system alarm. */
    deviceId: number | null;
    deviceName: string | null;
    propertyName: string;
    alarmSequenceState: AlarmSequenceState;
    alarmProcessCondition: AlarmProcessCondition;
    /** Time the alarm was triggered (ISO 8601). */
    createdOn: string;
    changedBy: string;
    /** Time the alarm was last changed (ISO 8601). */
    changedDate: string;
    ppcAlarmConfig: PpcAlarmConfigDto;
}

/** Filter of the alarm list, a criterion that is null matches every alarm. */
export interface PpcAlarmFilter {
    /** Period before now in hours in which the alarms were triggered. */
    triggeredWithinHours: number;
    severityLevel: AlarmSeverity | null;
    alarmProcessCondition: AlarmProcessCondition | null;
    alarmSequenceState: AlarmSequenceState | null;
    /** Part of the device name (case insensitive), empty for all alarms. */
    deviceName: string;
}
