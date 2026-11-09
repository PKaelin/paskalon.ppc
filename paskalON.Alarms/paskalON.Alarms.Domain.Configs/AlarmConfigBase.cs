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
    /// that the user interface maps to a language dependent text (see <see cref="Localizations.LocalizedText"/>).
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
        /// Time in milliseconds the value must stay abnormal before the alarm is raised.
        /// </summary>
        /// <remarks>
        /// - 0 raises the alarm immediately.
        /// - When the value is normal again before the time elapsed, the alarm is not raised and no action is sent
        ///   (e.g. a communication error of 200 milliseconds).
        /// - The raised alarm is created at the time the value became abnormal, not when the time elapsed.
        /// - The pending raise is only kept in memory, after a restart the time starts again with the first received value.
        /// </remarks>
        public long TimeBeforeRaisingAlarmMilliseconds
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, 0); field = value; }
        }


        /// <summary>
        /// Time in milliseconds the value must stay normal before the active alarm is normalized.
        /// </summary>
        /// <remarks>
        /// - 0 normalizes the alarm immediately.
        /// - When the value is abnormal again before the time elapsed, the alarm stays active and no action is sent,
        ///   so that short flickers back to normal do not normalize and raise the alarm again.
        /// - The alarm is normalized at the time the value became normal, not when the time elapsed.
        /// - The pending normalization is only kept in memory, after a restart the time starts again with the first received value.
        /// </remarks>
        public long TimeBeforeNormalizingAlarmMilliseconds
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, 0); field = value; }
        }


        /// <summary>
        /// Actions to be performed when the alarm is raised or normalized.
        /// </summary>
        /// <remarks>
        /// The trigger of the action defines whether it is a raise or normalize action.
        /// The actions are shared, the same action can be configured for many alarms.
        /// </remarks>
        public List<AlarmActionConfig> AlarmActions { get; set; } = new List<AlarmActionConfig>();


        /// <summary>
        /// Gets the property names and values that can be localization keys (e.g. {{PCS.OverTemp.Name}}).
        /// </summary>
        /// <returns>Name, Description, PossibleCauses, Response and Consequences with their values.</returns>
        public IReadOnlyList<KeyValuePair<string, string?>> GetLocalizableValues()
        {
            return new KeyValuePair<string, string?>[]
            {
                new KeyValuePair<string, string?>(nameof(Name), Name),
                new KeyValuePair<string, string?>(nameof(Description), Description),
                new KeyValuePair<string, string?>(nameof(PossibleCauses), PossibleCauses),
                new KeyValuePair<string, string?>(nameof(Response), Response),
                new KeyValuePair<string, string?>(nameof(Consequences), Consequences)
            };
        }
    }
}
