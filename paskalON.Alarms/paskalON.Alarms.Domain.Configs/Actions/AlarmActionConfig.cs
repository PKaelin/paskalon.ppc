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
    /// The name is the unique identifiable action name the recipient maps to its command.
    /// </remarks>
    public class AlarmActionConfig : NameBase
    {
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
