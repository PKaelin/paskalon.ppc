// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Messaging.Sequences;

namespace paskalON.OperatingModes.Dto.Powers
{
    /// <summary>
    /// Current system setpoints DTO.
    /// </summary>
    /// <remarks>
    /// The operating mode stamps the published setpoints with its source instance and sequence, so that the receivers drop outdated setpoints.
    /// Setpoints without a source instance are not stamped.
    /// </remarks>
    public class CurrentSetpointsDto : ISequencedMessage
    {
        /// <summary>
        /// Setpoint for active power in watts.
        /// </summary>
        public required double SetpointActivePowerWatts { get; set; }

        /// <summary>
        /// Setpoint for reactive power in vars.
        /// </summary>
        public required double SetpointReactivePowerVars { get; set; }


        /// <inheritdoc/>
        public string SourceInstance { get; init; } = string.Empty;


        /// <inheritdoc/>
        public long Sequence { get; init; }


        /// <inheritdoc/>
        public DateTimeOffset Timestamp { get; init; }
    }
}
