// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Dto
{
    /// <summary>
    /// Severity level of an alarm, analog to the alarm severity of the domain.
    /// </summary>
    public enum AlarmSeverityDto
    {
        /// <summary>
        /// Indicates an information that should be published.
        /// </summary>
        Info = 0,


        /// <summary>
        /// Indicates a potential problem or non-critical issue that needs attention.
        /// </summary>
        Warning = 1,


        /// <summary>
        /// Indicates a major problem that requires immediate action.
        /// </summary>
        Error = 2,


        /// <summary>
        /// Indicates a most severe issue that requires an urgent corrective action.
        /// </summary>
        Critical = 3
    }
}
