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
