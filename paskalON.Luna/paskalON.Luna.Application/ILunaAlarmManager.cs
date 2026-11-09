// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Dto.Summaries;

namespace paskalON.Luna.Application
{
    /// <summary>
    /// Keeps the alarm summary of the alarm service for the UI.
    /// </summary>
    public interface ILunaAlarmManager
    {
        /// <summary>
        /// Gets the latest alarm summary: the number of active alarms per severity level, unacknowledged and acknowledged separately.
        /// </summary>
        /// <returns>The latest alarm summary, null until the first summary was received from the alarm service.</returns>
        AlarmSummaryDto? GetSummary();


        /// <summary>
        /// Raised after a newer alarm summary was received, e.g. to push it to the UI clients.
        /// </summary>
        event EventHandler<AlarmSummaryDto>? SummaryChanged;
    }
}
