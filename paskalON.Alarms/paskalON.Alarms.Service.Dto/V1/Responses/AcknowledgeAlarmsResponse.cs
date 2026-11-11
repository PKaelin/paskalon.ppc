// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Service.Dto.V1.Responses
{
    /// <summary>
    /// Response class of an acknowledgement of alarms.
    /// </summary>
    public class AcknowledgeAlarmsResponse
    {
        /// <summary>
        /// Gets or sets the number of acknowledged alarms, the alarms that could not be acknowledged are not counted.
        /// </summary>
        public required int AcknowledgedCount { get; set; }
    }
}
