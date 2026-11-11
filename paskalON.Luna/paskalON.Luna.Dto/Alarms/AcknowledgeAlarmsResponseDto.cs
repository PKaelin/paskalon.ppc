// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Luna.Dto.Alarms
{
    /// <summary>
    /// Response to the UI of an acknowledgement of alarms.
    /// </summary>
    public record AcknowledgeAlarmsResponseDto
    {
        /// <summary>
        /// Number of acknowledged alarms, the alarms that could not be acknowledged are not counted.
        /// </summary>
        public required int AcknowledgedCount { get; init; }
    }
}
