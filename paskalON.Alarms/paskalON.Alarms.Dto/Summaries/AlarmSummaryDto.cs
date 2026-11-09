// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Dto.Summaries
{
    /// <summary>
    /// Number of active alarms per severity level, separated into unacknowledged and acknowledged alarms.
    /// </summary>
    /// <remarks>
    /// - The counts without prefix are the alarms in the sequence state Alarm (raised, not yet acknowledged).
    /// - The acknowledged counts are the alarms in the sequence state Acknowledged (still active, not yet normal).
    /// - Normalized alarms (sequence state Normal) are not counted.
    /// The summary is calculated by the alarm service, it is sent with every alarm action and returned by the alarm summary API.
    /// Recipients keep the summary with the latest <see cref="CalculatedOn"/>, an older summary (e.g. a delayed message) is ignored.
    /// </remarks>
    public record AlarmSummaryDto
    {
        /// <summary>
        /// Time the alarm service calculated the summary (UTC).
        /// </summary>
        public required DateTimeOffset CalculatedOn { get; init; }


        /// <summary>
        /// Number of unacknowledged alarms (sequence state Alarm) with the severity level Info.
        /// </summary>
        public required int InfoCount { get; init; }


        /// <summary>
        /// Number of unacknowledged alarms (sequence state Alarm) with the severity level Warning.
        /// </summary>
        public required int WarningCount { get; init; }


        /// <summary>
        /// Number of unacknowledged alarms (sequence state Alarm) with the severity level Error.
        /// </summary>
        public required int ErrorCount { get; init; }


        /// <summary>
        /// Number of unacknowledged alarms (sequence state Alarm) with the severity level Critical.
        /// </summary>
        public required int CriticalCount { get; init; }


        /// <summary>
        /// Number of acknowledged alarms (sequence state Acknowledged) with the severity level Info.
        /// </summary>
        public required int AcknowledgedInfoCount { get; init; }


        /// <summary>
        /// Number of acknowledged alarms (sequence state Acknowledged) with the severity level Warning.
        /// </summary>
        public required int AcknowledgedWarningCount { get; init; }


        /// <summary>
        /// Number of acknowledged alarms (sequence state Acknowledged) with the severity level Error.
        /// </summary>
        public required int AcknowledgedErrorCount { get; init; }


        /// <summary>
        /// Number of acknowledged alarms (sequence state Acknowledged) with the severity level Critical.
        /// </summary>
        public required int AcknowledgedCriticalCount { get; init; }
    }
}
