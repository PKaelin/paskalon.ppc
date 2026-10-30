// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Messaging.Sequences;

namespace paskalON.OperatingModes.Dto.Powers
{
    /// <summary>
    /// Current DER unit setpoints DTO.
    /// </summary>
    /// <remarks>
    /// The operating mode stamps the published setpoints with its source instance and sequence, so that the receivers drop outdated setpoints.
    /// Setpoints without a source instance are not stamped.
    /// </remarks>
    public record CurrentDerUnitSetpointsDto : ISequencedMessage
    {
        /// <summary>
        /// Gets or sets the name of the derived unit.
        /// </summary>
        /// <remarks>Required; must be a non-empty string.</remarks>
        public required string DerUnitName { get; set; }


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
