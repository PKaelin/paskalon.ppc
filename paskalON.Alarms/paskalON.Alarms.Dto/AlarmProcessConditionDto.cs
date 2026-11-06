// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Dto
{
    /// <summary>
    /// Condition of the process that caused the alarm event, analog to the alarm process condition of the domain.
    /// </summary>
    public enum AlarmProcessConditionDto
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
