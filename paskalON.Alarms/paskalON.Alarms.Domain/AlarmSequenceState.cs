// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Domain
{
    /// <summary>
    /// State of the alarm in the alarm sequence.
    /// </summary>
    /// <remarks>
    /// An unacknowledged alarm's icon blinks until the alarm is acknowledged.
    /// An acknowledged alarm's icon remains visible and steady (not blinking).
    /// </remarks>
    public enum AlarmSequenceState
    {
        /// <summary>
        /// The alarm is active and has not been acknowledged or reset.
        /// </summary>
        Alarm = 0,


        /// <summary>
        /// The alarm is active and has been acknowledged but has not been reset.
        /// </summary>
        Acknowledged = 1,


        /// <summary>
        /// The alarm has been reset and is not active.
        /// </summary>
        Normal = 2
    }
}
