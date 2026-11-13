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


        /// <summary>
        /// Supervisory active power setpoint in watts (e.g. of the operator) the applied setpoint is calculated from, null if unknown.
        /// </summary>
        public double? SupervisorySetpointActivePowerWatts { get; set; }


        /// <summary>
        /// Supervisory reactive power setpoint in vars (e.g. of the operator) the applied setpoint is calculated from, null if unknown.
        /// </summary>
        public double? SupervisorySetpointReactivePowerVars { get; set; }


        /// <summary>
        /// Why the applied active power setpoint differs from the supervisory setpoint, see <see cref="PowerLimitReasonDto"/>.
        /// </summary>
        /// <remarks>
        /// A number, so that the alarm service can evaluate it (it does not read enumerations).
        /// </remarks>
        public int ActivePowerLimitReason { get; set; }


        /// <summary>
        /// Why the applied reactive power setpoint differs from the supervisory setpoint, see <see cref="PowerLimitReasonDto"/>.
        /// </summary>
        /// <remarks>
        /// A number, so that the alarm service can evaluate it (it does not read enumerations).
        /// </remarks>
        public int ReactivePowerLimitReason { get; set; }


        /// <inheritdoc/>
        public string SourceInstance { get; init; } = string.Empty;


        /// <inheritdoc/>
        public long Sequence { get; init; }


        /// <inheritdoc/>
        public DateTimeOffset Timestamp { get; init; }
    }
}
