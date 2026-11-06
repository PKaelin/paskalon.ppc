// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Domain
{
    /// <summary>
    /// Condition of the process that caused the alarm event.
    /// </summary>
    /// <example>
    /// When an over temperature event occurs the condition is abnormal.
    /// When the temperature drops back to an acceptable level the condition reverts to normal.
    /// </example>
    public enum AlarmProcessCondition
    {
        /// <summary>
        /// The process is within its normal operating condition.
        /// </summary>
        Normal = 0,


        /// <summary>
        /// The process is outside its normal operating condition.
        /// </summary>
        Abnormal = 1
    }
}
