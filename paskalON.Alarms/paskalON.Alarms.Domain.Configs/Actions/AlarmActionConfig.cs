// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.Alarms.Domain.Configs.Actions
{
    /// <summary>
    /// Alarm action configuration that includes enough information for the recipient to execute the action without making another API call.
    /// </summary>
    /// <remarks>
    /// One action can be shared by many alarm configurations (e.g. RaiseAlarm for all communication errors).
    /// Alarm specific values are passed with the placeholders in the parameter values (e.g. {{AlarmId}}).
    /// Not derived from NameBase, because different actions can have the same name (e.g. ReducePower with different parameters).
    /// </remarks>
    public class AlarmActionConfig : DomainBase
    {
        /// <summary>
        /// Identifiable action name the recipient maps to its command.
        /// </summary>
        public required string Name
        {
            get;
            set { ArgumentException.ThrowIfNullOrWhiteSpace(value); field = value; }
        }


        /// <summary>
        /// Defines whether the action is performed when the alarm is raised or normalized.
        /// </summary>
        public required AlarmActionTrigger Trigger { get; set; }


        /// <summary>
        /// Key/Value parameters needed to execute the action.
        /// </summary>
        public List<AlarmActionParameterConfig> AlarmActionParameters { get; set; } = new List<AlarmActionParameterConfig>();
    }
}
