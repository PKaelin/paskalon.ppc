// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.PowerControls.Service.Dto.V1.Requests
{
    /// <summary>
    /// Set power setpoint DTO request.
    /// </summary>
    public class SetPowerSetpointRequest
    {
        /// <summary>
        /// Active power setpoint in watts to set for the system.
        /// </summary>
        public double ActivePowerWatt { get; set; }


        /// <summary>
        /// Reactive power setpoint in var to set for the system.
        /// </summary>
        public double ReactivePowerVar { get; set; }
    }
}
