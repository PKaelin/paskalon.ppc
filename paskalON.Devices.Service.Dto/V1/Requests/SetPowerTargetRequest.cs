// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Service.Dto.V1.Requests
{
    /// <summary>
    /// Set power target DTO request.
    /// </summary>
    public class SetPowerTargetRequest
    {
        /// <summary>
        /// Device ID of the PCS to set power target.
        /// </summary>
        public int DeviceId { get; set; }


        /// <summary>
        /// Active power target in watts to set for the PCS.
        /// </summary>
        public double ActivePowerWatt { get; set; }


        /// <summary>
        /// Reactive power target in var to set for the PCS.
        /// </summary>
        public double ReactivePowerVar { get; set; }
    }
}
