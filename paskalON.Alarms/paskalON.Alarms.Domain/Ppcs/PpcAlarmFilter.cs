// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Configs;

namespace paskalON.Alarms.Domain.Ppcs
{
    /// <summary>
    /// Filter of the PPC alarms, e.g. for the alarm list of the user interface.
    /// </summary>
    /// <remarks>
    /// All set criteria must match. A criterion that is null matches every alarm.
    /// </remarks>
    public record PpcAlarmFilter
    {
        /// <summary>
        /// Earliest creation timestamp of the alarms (inclusive).
        /// </summary>
        public required DateTimeOffset CreatedFrom { get; init; }


        /// <summary>
        /// Severity level of the alarm configuration, null for all severity levels.
        /// </summary>
        public AlarmSeverity? SeverityLevel { get; init; }


        /// <summary>
        /// Process condition of the alarms, null for all process conditions.
        /// </summary>
        public AlarmProcessCondition? AlarmProcessCondition { get; init; }


        /// <summary>
        /// Sequence state of the alarms, null for all sequence states.
        /// </summary>
        public AlarmSequenceState? AlarmSequenceState { get; init; }


        /// <summary>
        /// Part of the device name (case insensitive), null or empty for all alarms including the alarms without a device.
        /// </summary>
        public string? DeviceName { get; init; }
    }
}
