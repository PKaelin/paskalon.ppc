// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.PowerControls.Service.Dto.V1.Requests
{
    /// <summary>
    /// Set DER unit power setpoint DTO request.
    /// </summary>
    public record SetDerUnitPowerSetpointRequest
    {
        /// <summary>
        /// DER unit name to set the power setpoint for.
        /// </summary>
        public required string DerUnitName { get; set; }


        /// <summary>
        /// Active power setpoint in watts to set for the system.
        /// </summary>
        public required double ActivePowerWatt { get; set; }


        /// <summary>
        /// Reactive power setpoint in var to set for the system.
        /// </summary>
        public required double ReactivePowerVar { get; set; }
    }
}
