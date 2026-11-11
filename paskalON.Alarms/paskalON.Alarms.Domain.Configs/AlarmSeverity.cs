// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Domain.Configs
{
    /// <summary>
    /// Severity level of an alarm so that the seriousness of the alarm is present.
    /// </summary>
    /// <remarks>
    /// The values are ordered by seriousness, so that severities can be compared (e.g. severity >= Error).
    /// </remarks>
    public enum AlarmSeverity
    {
        /// <summary>
        /// Indicates an information that should be published. E.g. PCS vendor events.
        /// </summary>
        Info = 0,


        /// <summary>
        /// Indicates a potential problem or non-critical issue that needs attention. E.g. PCS warning states.
        /// </summary>
        Warning = 1,


        /// <summary>
        /// Indicates a major problem that requires immediate action. E.g. some PCS fault states.
        /// </summary>
        Error = 2,


        /// <summary>
        /// Indicates a most severe issue that requires an urgent corrective action. E.g. some PCS fault states.
        /// </summary>
        Critical = 3
    }
}
