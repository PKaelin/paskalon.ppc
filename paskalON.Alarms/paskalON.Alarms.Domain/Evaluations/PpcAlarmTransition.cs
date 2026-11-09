// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Domain.Evaluations
{
    /// <summary>
    /// Change of an alarm that waits for its delay to elapse.
    /// </summary>
    public enum PpcAlarmTransition
    {
        /// <summary>
        /// The value became abnormal, the alarm is raised after TimeBeforeRaisingAlarmMilliseconds.
        /// </summary>
        Raise = 0,


        /// <summary>
        /// The value became normal, the active alarm is normalized after TimeBeforeNormalizingAlarmMilliseconds.
        /// </summary>
        Normalize = 1
    }
}
