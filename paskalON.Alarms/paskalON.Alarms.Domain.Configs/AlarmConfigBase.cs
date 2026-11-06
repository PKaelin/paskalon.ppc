// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Configs.Actions;
using paskalON.Domains;

namespace paskalON.Alarms.Domain.Configs
{
    /// <summary>
    /// Base definition of every alarm configuration.
    /// </summary>
    /// <remarks>
    /// Name, Description, PossibleCauses, Response and Consequences are string identifiers (e.g. {{PCS.OverTemp.Name}}),
    /// that the user interface maps to a language dependent text.
    /// Not derived from NameBase, because the name is not unique: a device or dictionary entry configuration that overrides
    /// another configuration can use the same name.
    /// </remarks>
    public abstract class AlarmConfigBase : DomainBase
    {
        /// <summary>
        /// String identifier of the name of the alarm.
        /// </summary>
        public required string Name
        {
            get;
            set { ArgumentException.ThrowIfNullOrWhiteSpace(value); field = value; }
        }


        /// <summary>
        /// Indicates whether the alarm is enabled (active) or disabled (inactive).
        /// </summary>
        public required bool IsEnabled { get; set; }


        /// <summary>
        /// Unique identifier (error code) of the alarm.
        /// </summary>
        public required string AlarmId
        {
            get;
            set { ArgumentException.ThrowIfNullOrWhiteSpace(value); field = value; }
        }


        /// <summary>
        /// Severity level of the alarm.
        /// </summary>
        public required AlarmSeverity SeverityLevel { get; set; }


        /// <summary>
        /// String identifier of the short description of the alarm.
        /// </summary>
        public required string Description { get; set; }


        /// <summary>
        /// String identifier of the indication of possible causes for the alarm.
        /// </summary>
        public string? PossibleCauses { get; set; }


        /// <summary>
        /// String identifier that describes the appropriate response by the operator.
        /// </summary>
        public string? Response { get; set; }


        /// <summary>
        /// String identifier of the indication of what may occur if the alarm is ignored.
        /// </summary>
        public string? Consequences { get; set; }


        /// <summary>
        /// Delays the raise of the alarm by a certain time in milliseconds.
        /// </summary>
        /// <remarks>
        /// Value of 0 raises the alarm immediately.
        /// </remarks>
        public long TimeBeforeRaisingAlarmMilliseconds
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, 0); field = value; }
        }


        /// <summary>
        /// Actions to be performed when the alarm is raised or normalized.
        /// </summary>
        /// <remarks>
        /// The trigger of the action defines whether it is a raise or normalize action.
        /// </remarks>
        public List<AlarmActionConfig> AlarmActions { get; set; } = new List<AlarmActionConfig>();
    }
}
