// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Service.Dto.V1.Requests
{
    /// <summary>
    /// Request class for setting the operator setpoints of a maintenance override mode.
    /// </summary>
    public class SetOverrideSetpointRequest
    {
        /// <summary>
        /// Gets or sets the name of the maintenance override mode.
        /// </summary>
        public required string OperatingModeName { get; set; }


        /// <summary>
        /// Gets or sets the active power setpoint in Watts.
        /// </summary>
        public double ActivePowerWatts { get; set; }


        /// <summary>
        /// Gets or sets the reactive power setpoint in Vars.
        /// </summary>
        public double ReactivePowerVars { get; set; }
    }
}
