// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Dto.Powers
{
    /// <summary>
    /// Supervisory (operator) setpoints of the system DTO.
    /// </summary>
    /// <remarks>
    /// These are the setpoints set by the operator, not the setpoints applied after the operating mode stack (<see cref="CurrentSetpointsDto"/>).
    /// </remarks>
    public record SystemSetpointDto
    {
        /// <summary>
        /// Gets the active power setpoint in watts.
        /// </summary>
        public required double ActivePowerWatts { get; init; }


        /// <summary>
        /// Gets the reactive power setpoint in vars.
        /// </summary>
        public required double ReactivePowerVars { get; init; }
    }
}
