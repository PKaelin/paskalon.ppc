// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.Alarms.Domain.Configs.Actions
{
    /// <summary>
    /// Type independent Key/Value parameter configuration of an alarm action.
    /// </summary>
    public class AlarmActionParameterConfig : DomainBase
    {
        /// <summary>
        /// Parent relationship to AlarmActionConfig Id.
        /// </summary>
        public int AlarmActionConfigId { get; set; }


        /// <summary>
        /// Parent relationship to AlarmActionConfig.
        /// </summary>
        public required AlarmActionConfig AlarmActionConfig { get; set; }


        /// <summary>
        /// Unique key, per alarm action, of the parameter.
        /// </summary>
        public required string Key
        {
            get;
            set { ArgumentException.ThrowIfNullOrWhiteSpace(value); field = value; }
        }


        /// <summary>
        /// Value of the parameter key.
        /// </summary>
        /// <remarks>
        /// The value can also be a placeholder (e.g. {{DeviceId}}) that is replaced with the actual value during runtime.
        /// </remarks>
        public required string Value { get; set; }
    }
}
