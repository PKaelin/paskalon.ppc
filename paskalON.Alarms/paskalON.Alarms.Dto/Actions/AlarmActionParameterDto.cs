// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Dto.Actions
{
    /// <summary>
    /// Key/Value parameter of an alarm action.
    /// </summary>
    /// <remarks>
    /// The recipient validates the parameters for their correctness before executing the action.
    /// </remarks>
    public record AlarmActionParameterDto
    {
        /// <summary>
        /// Unique key, per alarm action, of the parameter.
        /// </summary>
        public required string Key { get; init; }


        /// <summary>
        /// Value of the parameter key, placeholders are already replaced with the actual values.
        /// </summary>
        public required string Value { get; init; }
    }
}
