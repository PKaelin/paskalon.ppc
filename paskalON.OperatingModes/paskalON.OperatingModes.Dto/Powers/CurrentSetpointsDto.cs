// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Dto.Powers
{
    /// <summary>
    /// Current system setpoints DTO.
    /// </summary>
    public class CurrentSetpointsDto
    {
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
