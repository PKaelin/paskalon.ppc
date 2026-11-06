// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Evaluations;

namespace paskalON.Alarms.Application.Subscribers
{
    /// <summary>
    /// Received published data waiting to be evaluated by the alarm handler.
    /// </summary>
    public record PpcAlarmMessage
    {
        /// <summary>
        /// Publisher of the data, the PPC alarm type and the device id.
        /// </summary>
        public required PpcAlarmKey Key { get; init; }


        /// <summary>
        /// Deserialized published data, e.g. a PcsDetailDto.
        /// </summary>
        public required object Data { get; init; }


        /// <summary>
        /// Time when the data was received.
        /// </summary>
        public required DateTimeOffset ReceivedOn { get; init; }
    }
}
