// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Ppcs;

namespace paskalON.Alarms.Domain.Evaluations
{
    /// <summary>
    /// Result of an alarm evaluation, the alarms that have to be persisted and published.
    /// </summary>
    public record PpcAlarmEvaluation
    {
        /// <summary>
        /// Newly raised alarms.
        /// </summary>
        public List<PpcAlarm> Raised { get; init; } = new List<PpcAlarm>();


        /// <summary>
        /// Existing alarms that got normalized.
        /// </summary>
        public List<PpcAlarm> Normalized { get; init; } = new List<PpcAlarm>();


        /// <summary>
        /// Indicates whether the evaluation raised or normalized any alarm.
        /// </summary>
        public bool HasChanges
        {
            get => Raised.Count > 0 || Normalized.Count > 0;
        }
    }
}
