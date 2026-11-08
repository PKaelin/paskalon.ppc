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
        /// Gets the latest alarm summary: the number of alarms in the sequence state Alarm per severity level.
        /// </summary>
        /// <returns>The latest alarm summary, null until the first summary was received from the alarm service.</returns>
        AlarmSummaryDto? GetSummary();
    }
}
