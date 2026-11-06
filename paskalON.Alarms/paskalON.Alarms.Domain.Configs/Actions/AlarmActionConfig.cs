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
    /// Not derived from NameBase, because the same action (e.g. ReducePower) can be configured for multiple alarms.
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
        /// Parent relationship to AlarmConfigBase Id.
        /// </summary>
        public int AlarmConfigBaseId { get; set; }


        /// <summary>
        /// Parent relationship to AlarmConfigBase.
        /// </summary>
        public required AlarmConfigBase AlarmConfigBase { get; set; }


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
