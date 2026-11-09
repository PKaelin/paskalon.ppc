// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Dto;
using paskalON.Alarms.Dto.Ppcs;
using paskalON.Alarms.Dto.Summaries;

namespace paskalON.Luna.Application
{
    /// <summary>
    /// Keeps the alarm summary of the alarm service for the UI and reads the alarm list of the alarm service.
    /// </summary>
    public interface ILunaAlarmManager
    {
        /// <summary>
        /// Gets the latest alarm summary: the number of active alarms per severity level, unacknowledged and acknowledged separately.
        /// </summary>
        /// <returns>The latest alarm summary, null until the first summary was received from the alarm service.</returns>
        AlarmSummaryDto? GetSummary();


        /// <summary>
        /// Gets the PPC alarms of the alarm service that were triggered within the period and match the other criteria.
        /// </summary>
        /// <param name="triggeredWithin">Period before now in which the alarms were created (triggered).</param>
        /// <param name="severityLevel">Severity level of the alarms, null for all severity levels.</param>
        /// <param name="processCondition">Process condition of the alarms, null for all process conditions.</param>
        /// <param name="sequenceState">Sequence state of the alarms, null for all sequence states.</param>
        /// <param name="deviceName">Part of the device name (case insensitive), null or empty for all alarms.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Matching alarms, the latest created alarm first.</returns>
        /// <exception cref="HttpRequestException">Thrown when the alarm service is not reachable or answers with an error (e.g. 503).</exception>
        Task<List<PpcAlarmDto>> GetPpcAlarmsAsync(TimeSpan triggeredWithin, AlarmSeverityDto? severityLevel, AlarmProcessConditionDto? processCondition,
            AlarmSequenceStateDto? sequenceState, string? deviceName, CancellationToken cancellationToken = default);


        /// <summary>
        /// Raised after a newer alarm summary was received, e.g. to push it to the UI clients.
        /// </summary>
        event EventHandler<AlarmSummaryDto>? SummaryChanged;
    }
}
