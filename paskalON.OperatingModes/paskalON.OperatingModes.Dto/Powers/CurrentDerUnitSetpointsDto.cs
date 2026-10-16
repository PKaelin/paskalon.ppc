// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Dto.Powers
{
    /// <summary>
    /// Current DER unit setpoints DTO.
    /// </summary>
    public record CurrentDerUnitSetpointsDto
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
    }
}
